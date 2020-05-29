using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using NSec.Cryptography;
using Rebex.Security.Cryptography;
using Texnomic.Chaos.NaCl;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using Texnomic.SecureDNS.Core.Options;
using Texnomic.SecureDNS.Core.Records;
using Texnomic.SecureDNS.Protocols.Extensions;
using Texnomic.SecureDNS.Serialization;

using Ed25519 = Texnomic.Chaos.NaCl.Ed25519;

namespace Texnomic.SecureDNS.Protocols
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-protocol/blob/master/DNSCRYPT-V2-PROTOCOL.txt"/>
    /// </summary>
    public sealed class DNSCrypt : Protocol
    {
        private readonly IPEndPoint IPEndPoint;

        private ICertificate Certificate;

        private readonly byte[] ClientPublicKey;
        private readonly byte[] ClientPrivateKey;

        private byte[] SharedKey;

        private readonly DNSCryptStamp Stamp;

        private readonly Random Random;

        private readonly IOptionsMonitor<DNSCryptOptions> Options;

        private bool IsInitialized;

        public DNSCrypt(IOptionsMonitor<DNSCryptOptions> DNSCryptOptions)
        {
            Options = DNSCryptOptions;

            Random = new Random();

            Stamp = DnSerializer.Deserialize(Options.CurrentValue.Stamp).Value as DNSCryptStamp;

            IPEndPoint = IPEndPoint.Parse(Stamp.Address);

            var ClientCurve25519 = new Curve25519();

            ClientPublicKey = ClientCurve25519.GetPublicKey();

            ClientPrivateKey = ClientCurve25519.GetPrivateKey();

            IsInitialized = false;
        }

        protected override async ValueTask InitializeAsync()
        {
            var Query = new Message()
            {
                ID = (ushort)Random.Next(),
                MessageType = MessageType.Query,
                Truncated = false,
                CheckingDisabled = true,
                RecursionDesired = true,
                Questions = new List<IQuestion>()
                {
                    new Question()
                    {
                        Type = RecordType.TXT,
                        Class = RecordClass.Internet,
                        Domain = Domain.FromString(Stamp.ProviderName)
                    }
                }
            };

            var SerializedQuery = DnSerializer.Serialize(Query);

            using var Client = new UdpClient();

            await Client.SendAsync(SerializedQuery, SerializedQuery.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            var SerializedAnswer = Task.IsCompletedSuccessfully ? Task.Result.Buffer : throw new TimeoutException();

            var AnswerMessage = DnSerializer.Deserialize(SerializedAnswer);

            var IsValid = VerifyServer(AnswerMessage);

            if (!IsValid)
                throw new CryptographicUnexpectedOperationException("Invalid Server Certificate.");

            SharedKey = MontgomeryCurve25519.KeyExchange(Certificate.PublicKey, ClientPrivateKey);

            IsInitialized = true;
        }

        private bool VerifyServer(IMessage Message)
        {
            foreach (var Answer in Message.Answers)
            {
                var Record = Answer.Record as TXT;

                Certificate = Record.Certificate;

                var Bytes = DnSerializer.Serialize(Certificate);

                var Result = Ed25519.Verify(Certificate.Signature, Bytes[73..], Stamp.PublicKey);

                if (Result) return true;
            }

            return false;
        }

        private static byte[] GenerateQueryPad(int QueryLength)
        {
            var Pad = Array.Empty<byte>();

            if (QueryLength < 256)
            {
                Pad = new byte[256 - QueryLength];
            }

            if (QueryLength > 256)
            {
                var PaddingLength = 256 + 64;

                while (PaddingLength < QueryLength)
                {
                    PaddingLength += 64;
                }

                Pad = new byte[PaddingLength - QueryLength];
            }

            Pad[0] = 0x80;

            return Pad;
        }

        private static T[] Concat<T>(params T[][] Arrays)
        {
            var Result = new T[Arrays.Sum(A => A.Length)];

            var Offset = 0;

            foreach (var Array in Arrays)
            {
                Array.CopyTo(Result, Offset);

                Offset += Array.Length;
            }

            return Result;
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            if (!IsInitialized) await InitializeAsync();

            var ClientNonce = RandomGenerator.Default.GenerateBytes(12);

            var ClientNoncePad = new byte[12];

            var PaddedClientNonce = Concat(ClientNonce, ClientNoncePad);

            var QueryPad = GenerateQueryPad(Query.Length);

            var PaddedQuery = Concat(Query, QueryPad);

            var EncryptedQuery = Encrypt(ref PaddedQuery, ref PaddedClientNonce);

            var QueryPacket = Concat(Certificate.ClientMagic, ClientPublicKey, ClientNonce, EncryptedQuery);

            using var Client = new UdpClient();

            await Client.SendAsync(QueryPacket, QueryPacket.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            var AnswerPacket = Task.IsCompletedSuccessfully ? Task.Result.Buffer : throw new TimeoutException();

            var ClientMagic = Encoding.ASCII.GetString(AnswerPacket[..8]);

            if (ClientMagic != "r6fnvWj8")
                throw new CryptographicUnexpectedOperationException("Invalid Client Magic Received.");

            if (!ClientNonce.SequenceEqual(AnswerPacket[8..20]))
                throw new CryptographicUnexpectedOperationException("Invalid Client Nonce Received.");

            var ServerNonce = AnswerPacket[20..32];

            var Nonce = Concat(ClientNonce, ServerNonce);

            var EncryptedAnswer = AnswerPacket[32..];

            var DecryptedAnswer = Decrypt(ref EncryptedAnswer, ref Nonce);

            if (DecryptedAnswer == null)
                throw new CryptographicUnexpectedOperationException("DNSCrypt Decryption Failed.");

            if (DecryptedAnswer.Length <= Query.Length)
                throw new CryptographicUnexpectedOperationException("DNSCrypt Decryption Failed.");

            return DecryptedAnswer;
        }

        private byte[] Encrypt(ref byte[] PaddedQuery, ref byte[] ClientNonce)
        {
            switch (Certificate.Version)
            {
                case ESVersion.X25519_XSalsa20Poly1305:

                    return XSalsa20Poly1305.Encrypt(PaddedQuery, SharedKey, ClientNonce);

                case ESVersion.X25519_XChacha20Poly1305:

                    var XChaCha20Poly1305 = new XChaCha20Poly1305(SharedKey);

                    var CipherText = new byte[PaddedQuery.Length];

                    XChaCha20Poly1305.Encrypt(ClientNonce, PaddedQuery, CipherText);

                    return CipherText;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ESVersion));
            }
        }

        private byte[] Decrypt(ref byte[] EncryptedAnswer, ref byte[] ServerNonce)
        {
            switch (Certificate.Version)
            {
                case ESVersion.X25519_XSalsa20Poly1305:

                    return XSalsa20Poly1305.TryDecrypt(EncryptedAnswer, SharedKey, ServerNonce);

                case ESVersion.X25519_XChacha20Poly1305:

                    var XChaCha20Poly1305 = new XChaCha20Poly1305(SharedKey);

                    var PlainText = new byte[EncryptedAnswer.Length];

                    XChaCha20Poly1305.Decrypt(ServerNonce, EncryptedAnswer, PlainText);

                    return PlainText;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ESVersion));
            }
        }

        protected override void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {

            }

            IsDisposed = true;
        }
    }
}

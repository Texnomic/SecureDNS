using Microsoft.Extensions.Options;
using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chaos.NaCl;
using Rebex.Security.Cryptography;
using Texnomic.Chaos.NaCl;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Options;
using Texnomic.DNS.Records;

using Ed25519 = Rebex.Security.Cryptography.Ed25519;

namespace Texnomic.DNS.Protocols
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-protocol/blob/master/DNSCRYPT-V2-PROTOCOL.txt"/>
    /// </summary>
    public sealed class DNSCrypt : Protocol
    {
        private readonly IPEndPoint IPEndPoint;

        private readonly UdpClient Client;

        private readonly ChaCha20Poly1305 ChaCha20Poly1305;

        private Certificate Certificate;

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

            Client = new UdpClient
            {
                Client =
                {
                    SendTimeout = Options.CurrentValue.Timeout,
                    ReceiveTimeout = Options.CurrentValue.Timeout
                }
            };

            Stamp = (DNSCryptStamp)Options.CurrentValue.Stamp.Value;

            IPEndPoint = IPEndPoint.Parse(Stamp.Address);

            var ClientCurve25519 = new Curve25519();

            ClientPublicKey = ClientCurve25519.GetPublicKey();

            ClientPrivateKey = ClientCurve25519.GetPrivateKey();

            ChaCha20Poly1305 = new ChaCha20Poly1305();

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

            var SerializedQuery = await BinarySerializer.SerializeAsync(Query);

            await Client.SendAsync(SerializedQuery, SerializedQuery.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            var SerializedAnswer = Task.IsCompleted ? Task.Result.Buffer : throw new TimeoutException();

            var AnswerMessage = await BinarySerializer.DeserializeAsync<Message>(SerializedAnswer);

            var IsValid = await VerifyServer(AnswerMessage);

            if (!IsValid)
                throw new CryptographicUnexpectedOperationException("Invalid Server Certificate.");

            SharedKey = MontgomeryCurve25519.KeyExchange(Certificate.PublicKey, ClientPrivateKey);

            IsInitialized = true;
        }

        private async ValueTask<bool> VerifyServer(IMessage Message)
        {
            var Record = (TXT)Message.Answers[0].Record;

            Certificate = Record.Certificate;

            var Bytes = await BinarySerializer.SerializeAsync(Certificate);

            var Ed25519 = new Ed25519();

            Ed25519.FromPublicKey(Stamp.PublicKey.Value);

            return Ed25519.VerifyMessage(Bytes[72..], Certificate.Signature);
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

            await Client.SendAsync(QueryPacket, QueryPacket.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            var AnswerPacket = Task.IsCompleted ? Task.Result.Buffer : throw new TimeoutException();

            var ClientMagic = Encoding.ASCII.GetString(AnswerPacket[..8]);

            if (ClientMagic != "r6fnvWj8")
                throw new CryptographicUnexpectedOperationException("Invalid Client Magic Received.");

            if (!ClientNonce.SequenceEqual(AnswerPacket[8..20]))
                throw new CryptographicUnexpectedOperationException("Invalid Client Nonce Received.");

            var ServerNonce = AnswerPacket[20..32];

            var Nonce = Concat(ClientNonce, ServerNonce);

            var EncryptedAnswer = AnswerPacket[32..];

            var DecryptedAnswer = Decrypt(ref EncryptedAnswer, ref Nonce);

            if(DecryptedAnswer == null) 
                throw new CryptographicUnexpectedOperationException("DNSCrypt Decryption Failed.");

            DecryptedAnswer = DecryptedAnswer.TrimEnd();

            //TODO Retry ?
            //DecryptedAnswer = DecryptedAnswer.Length > Query.Length + 1 ? DecryptedAnswer : await ResolveAsync(Query);

            return DecryptedAnswer;
        }

        private static string ByteArrayToString(byte[] Data)
        {
            return BitConverter.ToString(Data).Replace("-", ", 0x");
        }

        private byte[] Encrypt(ref byte[] PaddedQuery, ref byte[] PaddedClientNonce)
        {
            switch (Certificate.Version)
            {
                case ESVersion.X25519_XSalsa20Poly1305:

                    return XSalsa20Poly1305.Encrypt(PaddedQuery, SharedKey, PaddedClientNonce);

                case ESVersion.X25519_XChacha20Poly1305:

                    var SKey = Key.Import(KeyAgreementAlgorithm.X25519, SharedKey, KeyBlobFormat.RawSymmetricKey);

                    var Nonce = new Nonce(PaddedClientNonce, 0);

                    return ChaCha20Poly1305.Encrypt(SKey, in Nonce, null, PaddedQuery);

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

                    var SKey = Key.Import(KeyAgreementAlgorithm.X25519, SharedKey, KeyBlobFormat.RawSymmetricKey);

                    var Nonce = new Nonce(ServerNonce, 0);

                    ChaCha20Poly1305.Decrypt(SKey, in Nonce, null, EncryptedAnswer, out var Answer);

                    return Answer;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ESVersion));
            }
        }

        protected override void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                Client.Dispose();
            }

            IsDisposed = true;
        }
    }
}

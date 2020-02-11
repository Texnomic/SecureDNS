using BinarySerialization;
using Microsoft.Extensions.Options;
using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Options;
using Texnomic.DNS.Records;

namespace Texnomic.DNS.Protocols
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-protocol/blob/master/DNSCRYPT-V2-PROTOCOL.txt"/>
    /// </summary>
    public class DNSCrypt : IProtocol
    {
        private IPEndPoint IPEndPoint;
        private UdpClient Client;

        private Key ClientKey;
        private Certificate ServerCertificate;
        private SharedSecret SharedSecret;
        private Key SharedKey;

        private DNSCryptStamp Stamp;

        private readonly Random Random;
        private readonly IOptionsMonitor<DNSCryptOptions> Options;
        private readonly BinarySerializer BinarySerializer;

        public DNSCrypt(IOptionsMonitor<DNSCryptOptions> DNSCryptOptions)
        {
            Options = DNSCryptOptions;

            Random = new Random();

            BinarySerializer = new BinarySerializer();
        }


        public async ValueTask Initialize()
        {
            Stamp = (DNSCryptStamp)Options.CurrentValue.Stamp.Value;

            IPEndPoint = IPEndPoint.Parse(Stamp.Address);

            Client = new UdpClient
            {
                Client =
                {
                    SendTimeout = Options.CurrentValue.Timeout,
                    ReceiveTimeout = Options.CurrentValue.Timeout
                }
            };

            var SessionMessage = new Message()
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
                        Domain = new Domain(Stamp.ProviderName)
                    }
                }
            };

            var AnswerMessage = await ResolveAsync(SessionMessage);

            var IsVerified = await VerifyServer(AnswerMessage);

            CreateKeys();

            var QueryMessage = new Message()
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
                        Type = RecordType.A,
                        Class = RecordClass.Internet,
                        Domain = (Domain)"facebook.com"
                    }
                }
            };

            var Bytes = BinarySerializer.Serialize(QueryMessage);

            var EQB = CreateDNSCryptQuery(Bytes);

            var Result = await ResolveAsync(EQB);
        }


        private async ValueTask<bool> VerifyServer(IMessage Message)
        {
            var Record = (TXT)Message.Answers[0].Record;

            ServerCertificate = Record.Certificate;

            var ServerPublicKey = PublicKey.Import(SignatureAlgorithm.Ed25519, Stamp.PublicKey, KeyBlobFormat.RawPublicKey);

            var RecordBytes = await BinarySerializer.SerializeAsync(Record);

            return SignatureAlgorithm.Ed25519.Verify(ServerPublicKey, RecordBytes.Skip(73).ToArray(), Record.Certificate.Signature);
        }

        private void CreateKeys()
        {
            var ServerPublicKey = PublicKey.Import(KeyAgreementAlgorithm.X25519, Stamp.PublicKey, KeyBlobFormat.RawPublicKey);

            ClientKey = RandomGenerator.Default.GenerateKey(KeyAgreementAlgorithm.X25519);

            SharedSecret = KeyAgreementAlgorithm.X25519.Agree(ClientKey, ServerPublicKey);

            SharedKey = KeyDerivationAlgorithm.HkdfSha256.DeriveKey(SharedSecret, null, null, AeadAlgorithm.ChaCha20Poly1305);
        }

        /// <summary>
        /// <dnscrypt-query> ::= <client-magic> <client-pk> <client-nonce> <encrypted-query>
        /// </summary>
        /// <returns></returns>
        private byte[] CreateDNSCryptQuery(ReadOnlySpan<byte> Query)
        {
            var QueryNonce = GenerateQueryNonce();

            return ServerCertificate.ClientMagic
                                    .Concat(ClientKey.Export(KeyBlobFormat.RawPublicKey))
                                    .Concat(QueryNonce.ToArray())
                                    .Concat(CreateEncryptedQuery(QueryNonce, Query))
                                    .ToArray();
        }

        /// <summary>
        /// <encrypted-query> ::= AE(<shared-key> <client-nonce> <client-nonce-pad>, <client-query> <client-query-pad>)
        /// AE ::= the authenticated encryption algorithm.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<byte> CreateEncryptedQuery(Nonce QueryNonce, ReadOnlySpan<byte> Query)
        {
            return AeadAlgorithm.ChaCha20Poly1305.Encrypt(SharedKey, QueryNonce, null,
                SharedKey.Export(KeyBlobFormat.RawPublicKey)
                            .Concat(QueryNonce.ToArray())
                            .Concat(GenerateQueryNoncePad())
                            .Concat(Query.ToArray())
                            .Concat(GenerateQueryPad(Query.Length))
                            .ToArray());
        }

        /// <summary>
        /// <min-query-len> is a variable length, initially set to 256 bytes,
        /// and must be a multiple of 64 bytes.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<byte> GenerateQueryPad(int QueryLength)
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

        /// <summary>
        /// When using X25519-XSalsa20Poly1305, this construction requires a 24 bytes
        /// nonce, that must not be reused for a given shared secret.
        /// 
        /// With a 24 bytes nonce, a question sent by a DNSCrypt client must be
        /// encrypted using the shared secret, and a nonce constructed as follows:
        /// 12 bytes chosen by the client followed by 12 NUL (0) bytes.
        /// 
        /// A response to this question must be encrypted using the shared secret,
        /// and a nonce constructed as follows: the bytes originally chosen by
        /// the client, followed by bytes chosen by the resolver.
        /// 
        /// The resolver's half of the nonce should be randomly chosen.
        /// 
        /// The client's half of the nonce can include a timestamp in addition to a
        /// counter or to random bytes, so that when a response is received, the
        /// client can use this timestamp to immediately discard responses to
        /// queries that have been sent too long ago, or dated in the future.
        /// </summary>
        /// <returns></returns>
        private static Nonce GenerateQueryNonce(int Length = 12)
        {
            return new Nonce(RandomGenerator.Default.GenerateBytes(Length), 0);
        }

        /// <summary>
        /// <client-nonce-pad> ::= <client-nonce> length is half the nonce length
        /// required by the encryption algorithm.In client queries, the other half,
        /// <client-nonce-pad> is filled with NUL bytes.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<byte> GenerateQueryNoncePad(int Length = 12)
        {
            return new byte[Length];
        }

        public byte[] Resolve(byte[] Query)
        {
            Client.Send(Query, Query.Length, IPEndPoint);

            return Client.Receive(ref IPEndPoint);
        }

        public IMessage Resolve(IMessage Query)
        {
            var Buffer = BinarySerializer.Serialize(Query);

            Client.Send(Buffer, Buffer.Length, IPEndPoint);

            Buffer = Client.Receive(ref IPEndPoint);

            return BinarySerializer.Deserialize<Message>(Buffer);
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            await Client.SendAsync(Query, Query.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            var Result = Task.IsCompleted ? Task.Result : throw new TimeoutException();

            return Result.Buffer;
        }

        public async Task<IMessage> ResolveAsync(IMessage Query)
        {
            var Buffer = await BinarySerializer.SerializeAsync(Query);

            await Client.SendAsync(Buffer, Buffer.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            var Result = Task.IsCompleted ? Task.Result : throw new TimeoutException();

            return await BinarySerializer.DeserializeAsync<Message>(Result.Buffer);
        }

        private bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                Client.Dispose();
            }

            IsDisposed = true;
        }

        ~DNSCrypt()
        {
            Dispose(false);
        }
    }
}

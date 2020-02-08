using BinarySerialization;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexConvertors.Extensions;
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
    public class DNSCrypt : IProtocol
    {
        private IPEndPoint IPEndPoint;
        private UdpClient Client;

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
            IPEndPoint = IPEndPoint.Parse(Options.CurrentValue.Stamp.Hostname);

            Client = new UdpClient
            {
                Client =
                {
                    SendTimeout = Options.CurrentValue.Timeout,
                    ReceiveTimeout = Options.CurrentValue.Timeout
                }
            };

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
                        Type = RecordType.TXT,
                        Class = RecordClass.Internet,
                        Domain = new Domain(Options.CurrentValue.Stamp.ProviderName)
                    }
                }
            };

            var AnswerMessage = await ResolveAsync(QueryMessage);

            var Record = (TXT)AnswerMessage.Answers[0].Record;

            var Algorithm = new Ed25519();

            var PKey = PublicKey.Import(Algorithm, Options.CurrentValue.Stamp.PublicKey, KeyBlobFormat.RawPublicKey);

            var RecordBytes = await BinarySerializer.SerializeAsync(Record);

            var Valid = Algorithm.Verify(PKey, RecordBytes.Skip(73).ToArray(), Record.Certificate.Signature);

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

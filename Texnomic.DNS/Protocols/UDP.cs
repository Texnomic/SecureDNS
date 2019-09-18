using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Protocols
{
    public class UDP : IProtocol
    {
        private IPEndPoint IPEndPoint;
        private readonly UdpClient Client;
        private readonly BinarySerializer BinarySerializer;
        private const int Timeout = 2000;

        public UDP(IPAddress IPAddress)
        {
            BinarySerializer = new BinarySerializer();
            IPEndPoint = new IPEndPoint(IPAddress, 53);
            Client = new UdpClient
            {
                Client =
                {
                    SendTimeout = Timeout,
                    ReceiveTimeout = Timeout
                }
            };
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

            Task.Wait(Timeout);

            var Result = Task.IsCompleted ? Task.Result : throw new TimeoutException();

            return Result.Buffer;
        }

        public async Task<IMessage> ResolveAsync(IMessage Query)
        {
            var Buffer = await BinarySerializer.SerializeAsync(Query);

            await Client.SendAsync(Buffer, Buffer.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Timeout);

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

        ~UDP()
        {
            Dispose(false);
        }
    }
}

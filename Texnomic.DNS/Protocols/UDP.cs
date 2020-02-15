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
        private readonly IPEndPoint IPEndPoint;
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
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public IMessage Resolve(IMessage Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            //Query = PrefixLength(ref Query);

            await Client.SendAsync(Query, Query.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Timeout);

            var Result = Task.IsCompleted ? Task.Result : throw new TimeoutException();

            return Result.Buffer;
        }

        public async Task<IMessage> ResolveAsync(IMessage Query)
        {
            var Buffer = await BinarySerializer.SerializeAsync(Query);

            //Buffer = PrefixLength(ref Buffer);

            await Client.SendAsync(Buffer, Buffer.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Timeout);

            var Result = Task.IsCompleted ? Task.Result : throw new TimeoutException();

            return await BinarySerializer.DeserializeAsync<Message>(Result.Buffer);
        }

        private static byte[] PrefixLength(ref byte[] Query)
        {
            var Length = BitConverter.GetBytes((ushort)Query.Length);

            Array.Reverse(Length);

            var Buffer = new byte[Query.Length + 2];

            Array.Copy(Length, Buffer, 2);

            Array.Copy(Query, 0, Buffer, 2, Query.Length);

            return Buffer;
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

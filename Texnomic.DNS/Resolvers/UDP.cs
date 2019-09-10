using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public class UDP : IResolver
    {
        private IPEndPoint IPEndPoint;
        private readonly UdpClient Client;

        public UDP(IPAddress IPAddress)
        {
            IPEndPoint = new IPEndPoint(IPAddress, 53);
            Client = new UdpClient
            {
                Client =
                {
                    SendTimeout = 2000,
                    ReceiveTimeout = 2000
                }
            };
        }

        public byte[] Resolve(byte[] Query)
        {
            Client.Send(Query, Query.Length, IPEndPoint);

            return Client.Receive(ref IPEndPoint);
        }

        public Message Resolve(Message Query)
        {
            var Buffer = Query.ToArray();

            Client.Send(Buffer, Buffer.Length, IPEndPoint);

            Buffer = Client.Receive(ref IPEndPoint);

            return Message.FromArray(Buffer);
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            await Client.SendAsync(Query, Query.Length, IPEndPoint);

            var Result = await Client.ReceiveAsync();

            return Result.Buffer;
        }

        public async Task<Message> ResolveAsync(Message Query)
        {
            var Buffer = Query.ToArray();

            await Client.SendAsync(Buffer, Buffer.Length, IPEndPoint);

            var Result = await Client.ReceiveAsync();

            return Message.FromArray(Result.Buffer);
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

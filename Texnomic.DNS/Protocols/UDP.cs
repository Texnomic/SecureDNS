using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Options;

namespace Texnomic.DNS.Protocols
{
    public class UDP : Protocol
    {
        private readonly IOptionsMonitor<UDPOptions> Options;

        private UdpClient Client;

        private IPEndPoint IPEndPoint;

        public UDP(IOptionsMonitor<UDPOptions> UDPOptions)
        {
            Options = UDPOptions;

            Client = new UdpClient
            {
                Client =
                {
                    SendTimeout = Options.CurrentValue.Timeout,
                    ReceiveTimeout = Options.CurrentValue.Timeout
                }
            };

            IPEndPoint = new IPEndPoint(IPAddress.Parse(Options.CurrentValue.Host), Options.CurrentValue.Port);
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            await Client.SendAsync(Query, Query.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            var Result = Task.IsCompleted ? Task.Result : throw new TimeoutException();

            return Result.Buffer;
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

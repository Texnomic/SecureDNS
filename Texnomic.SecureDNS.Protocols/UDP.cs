using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Texnomic.SecureDNS.Core.Options;
using Texnomic.SecureDNS.Protocols.Extensions;

namespace Texnomic.SecureDNS.Protocols
{
    public class UDP : Protocol
    {
        private readonly IOptionsMonitor<UDPOptions> Options;

        private UdpClient Client;

        private readonly IPEndPoint IPEndPoint;

        private readonly SemaphoreSlim SemaphoreSlim;


        public UDP(IOptionsMonitor<UDPOptions> UDPOptions)
        {
            Options = UDPOptions;

            Client = new UdpClient();

            IPEndPoint = new IPEndPoint(IPAddress.Parse(Options.CurrentValue.Host), Options.CurrentValue.Port);

            SemaphoreSlim = new SemaphoreSlim(1);
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            await SemaphoreSlim.WaitAsync();

            await Client.SendAsync(Query, Query.Length, IPEndPoint);

            //Note: UDPClient Async Doesn't Support Timeout.

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            if (Task.IsCompletedSuccessfully)
            {
                SemaphoreSlim.Release();

                return Task.Result.Buffer;
            }

            //Note: UPDClient Stops Responding After Throwing TimeoutException.

            Client.Dispose();

            Client = new UdpClient();

            SemaphoreSlim.Release();

            throw new TimeoutException();
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

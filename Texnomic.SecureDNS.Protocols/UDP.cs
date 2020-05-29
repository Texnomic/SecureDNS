using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Texnomic.SecureDNS.Core.Options;

namespace Texnomic.SecureDNS.Protocols
{
    public class UDP : Protocol
    {
        private readonly IOptionsMonitor<UDPOptions> Options;

        private IPEndPoint IPEndPoint;

        public UDP(IOptionsMonitor<UDPOptions> Options)
        {
            this.Options = Options;

            this.Options.OnChange(OptionsOnChange);

            IPEndPoint = new IPEndPoint(IPAddress.Parse(Options.CurrentValue.Host), Options.CurrentValue.Port);
        }

        private void OptionsOnChange(UDPOptions UDPOptions)
        {
            IPEndPoint = new IPEndPoint(IPAddress.Parse(UDPOptions.Host), UDPOptions.Port);
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            using var Client = new UdpClient();

            await Client.SendAsync(Query, Query.Length, IPEndPoint);

            var Task = Client.ReceiveAsync();

            Task.Wait(Options.CurrentValue.Timeout);

            return Task.IsCompletedSuccessfully ? Task.Result.Buffer : throw new TimeoutException();
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

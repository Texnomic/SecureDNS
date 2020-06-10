using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Servers.Proxy;
using Texnomic.SecureDNS.Terminal.Options;

namespace Texnomic.SecureDNS.Terminal
{
    public class CLI : IHostedService, IDisposable
    {
        private readonly IOptionsMonitor<TerminalOptions> Options;

        private readonly UDPServer2 UDPServer;

        private readonly TCPServer2 TCPServer;


        public CLI(IOptionsMonitor<TerminalOptions> TerminalOptions, UDPServer2 UDPServer, TCPServer2 TCPServer)
        {
            Options = TerminalOptions;

            this.UDPServer = UDPServer;

            this.TCPServer = TCPServer;
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            await UDPServer.StartAsync(CancellationToken);

            await TCPServer.StartAsync(CancellationToken);
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {
            await UDPServer.StopAsync(CancellationToken);

            await TCPServer.StopAsync(CancellationToken);
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
                UDPServer.Dispose();
                TCPServer.Dispose();
            }

            IsDisposed = true;
        }

        ~CLI()
        {
            Dispose(false);
        }
    }
}

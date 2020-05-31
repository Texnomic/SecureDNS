using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Texnomic.DNS.Servers;
using Texnomic.SecureDNS.Terminal.Options;

namespace Texnomic.SecureDNS.Terminal
{
    public class CLI : IHostedService, IDisposable
    {
        private readonly IOptionsMonitor<TerminalOptions> Options;

        private readonly ProxyServer Server;


        public CLI(IOptionsMonitor<TerminalOptions> TerminalOptions, ProxyServer ProxyServer)
        {
            Options = TerminalOptions;

            Server = ProxyServer;
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            await Server.StartAsync(CancellationToken);
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {
            await Server.StopAsync(CancellationToken);
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
                Server.Dispose();
            }

            IsDisposed = true;
        }

        ~CLI()
        {
            Dispose(false);
        }
    }
}

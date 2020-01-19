using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Texnomic.DNS.Servers;
using Texnomic.SecureDNS.Terminal.Options;

using Console = Colorful.Console;

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
            Server.Started += (Sender, Args) => Console.WriteLine(" Server Started.\n\r");
            Server.Stopped += (Sender, Args) => Console.WriteLine("\n\r Server Stopped.");

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

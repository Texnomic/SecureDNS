using Destructurama;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Texnomic.DNS.Servers;
using Texnomic.SecureDNS.Terminal.Options;

namespace Texnomic.SecureDNS.Terminal
{
    public class TerminalCMD : IHostedService, IDisposable
    {
        private readonly TerminalOptions Options;

        private readonly ProxyServer Server;


        public TerminalCMD(IOptionsMonitor<TerminalOptions> TerminalOptions, ProxyServer ProxyServer)
        {
            Options = TerminalOptions.CurrentValue;

            Server = ProxyServer;
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            Log.Logger = new LoggerConfiguration()
                            .Destructure.UsingAttributes()
                            .WriteTo.Seq(Options.SeqUriEndPoint, compact: true)
                            .CreateLogger();

            Server.Started += (Sender, Args) => Console.WriteLine("Server Started.");
            Server.Stopped += (Sender, Args) => Console.WriteLine("Server Stopped.");
            Server.Errored += (Sender, Args) => Console.WriteLine($"Server Error: {Args.Error.Message}.");

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

        ~TerminalCMD()
        {
            Dispose(false);
        }
    }
}

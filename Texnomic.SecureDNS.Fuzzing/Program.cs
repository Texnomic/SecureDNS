using System;
using System.Threading;
using System.Threading.Tasks;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.ResponsibilityChain;

namespace Texnomic.SecureDNS.Fuzzing
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                            //.WriteTo.Console()
                            .WriteTo.Seq("http://127.0.0.1:5341", apiKey: "PFscdeWf391ACwwiPCvy")
                            .CreateLogger();

                var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
                var ServerResponsibilityChain = new ProxyResponsibilityChain(ActivatorMiddlewareResolver);
                var Server = new ProxyServer(ServerResponsibilityChain, Log.Logger, 8);
                await Server.StartAsync(new CancellationToken());

                Console.WriteLine("Server Started");

                Console.ReadLine();
            }
            catch (Exception Error)
            {
                Console.WriteLine($"Server Died. {Error.Message}");

                Console.ReadLine();
            }
        }
    }
}

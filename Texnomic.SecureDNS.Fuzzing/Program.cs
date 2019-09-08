using System;
using System.Threading;
using System.Threading.Tasks;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.ResponsibilityChain;
using Texnomic.DNS.Servers;

namespace Texnomic.SecureDNS.Fuzzing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
                var ProxyResponsibilityChain = new ProxyResponsibilityChain(ActivatorMiddlewareResolver);
                var Server = new SimpleServer(ProxyResponsibilityChain);

                await Server.StartAsync(new CancellationToken());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }

        }
    }
}

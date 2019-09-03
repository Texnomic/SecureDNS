using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.ResponsibilityChain;
using Texnomic.DNS.Servers;

namespace Texnomic.DNS.Tests.Servers
{
    [TestClass]
    public class Proxy
    {
        private ProxyServer Server;

        [TestInitialize]
        public void Initialize()
        {
            var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
            var ProxyResponsibilityChain = new ProxyResponsibilityChain(ActivatorMiddlewareResolver);
            Server = new ProxyServer(ProxyResponsibilityChain, 1);
        }

        [TestMethod]
        public async Task Start()
        {
            await Server.StartAsync(new CancellationToken());
        }

    }
}

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Servers.ResponsibilityChain;

namespace Texnomic.DNS.Servers.Tests
{
    [TestClass]
    public class Proxy
    {
        private SimpleServer Server;

        [TestInitialize]
        public void Initialize()
        {
            //Stop-Service -DisplayName 'Docker Desktop Service'
            //Stop-Service -DisplayName 'Internet Connection Sharing (ICS)'

            var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
            var ProxyResponsibilityChain = new ServerResponsibilityChain(ActivatorMiddlewareResolver);
            Server = new SimpleServer(ProxyResponsibilityChain);
        }

        [TestMethod]
        public async Task Start()
        {
            await Server.StartAsync(new CancellationToken());
        }
    }
}

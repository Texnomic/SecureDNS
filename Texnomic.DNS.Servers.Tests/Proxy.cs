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
            //Set-Service -Name 'Internet Connection Sharing (ICS)' -StartupType Disabled
            //Stop-Service -DisplayName 'Internet Connection Sharing (ICS)'
            //Resolve-DnsName -Name mail.google.com -Server 127.0.0.1 -Type A

            var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
            var ProxyResponsibilityChain = new ProxyResponsibilityChain(ActivatorMiddlewareResolver);
            Server = new SimpleServer(ProxyResponsibilityChain);
        }

        [TestMethod]
        public async Task Start()
        {
            await Server.StartAsync(new CancellationToken());
        }
    }
}

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Texnomic.Socks5.Options;
using Texnomic.Socks5.WebProxy;
using Texnomic.Socks5.WebProxy.Options;

namespace Texnomic.SecureDNS.Tests.Tor
{
    [TestClass]
    public class TorProxy
    {
        [TestMethod]
        public async Task Proxy()
        {
            var Socks5Options = new Socks5Options();
            var Socks5WebProxyOptions = new Socks5WebProxyOptions();

            var Socks5OptionsMonitor = Mock.Of<IOptionsMonitor<Socks5Options>>(Options => Options.CurrentValue == Socks5Options);
            var Socks5WebProxyOptionsMonitor = Mock.Of<IOptionsMonitor<Socks5WebProxyOptions>>(Options => Options.CurrentValue == Socks5WebProxyOptions);

            var Proxy = new Socks5WebProxy(Socks5WebProxyOptionsMonitor, Socks5OptionsMonitor);

            await Proxy.StartAsync(CancellationToken.None);

            var Client = new WebClient
            {
                Proxy = new WebProxy(new Uri("http://127.0.0.1:8000"))
            };

            var TorIP = await Client.DownloadStringTaskAsync("https://httpbin.org/ip");

            var Cloudflare = await Client.DownloadStringTaskAsync("http://dns4torpnlfs2ifuz2s2yf3fc7rdmsbhm6rw75euj35pac6ap25zgqad.onion");
        }
    }
}

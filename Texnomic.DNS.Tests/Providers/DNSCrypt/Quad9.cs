using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Texnomic.DNS.Options;
using Moq;

namespace Texnomic.DNS.Tests.Providers.DNSCrypt
{
    [TestClass]
    public class Quad9
    {
        [TestMethod]
        public async Task QueryAsync()
        {
            var Options = new DNSCryptOptions();

            var OptionsMonitor = Mock.Of<IOptionsMonitor<DNSCryptOptions>>(Opt => Opt.CurrentValue == Options);

            var DNSCrypt = new Protocols.DNSCrypt(OptionsMonitor);

            //await DNSCrypt.Initialize();
        }
    }
}

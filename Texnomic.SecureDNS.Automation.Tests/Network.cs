using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Texnomic.SecureDNS.Automation.Tests
{
    [TestClass]
    public class Network
    {
        [TestClass]
        public class Adapters
        {
            [TestMethod]
            public async Task Get()
            {
                var Adapters = await Automation.Network.Adapters.Get();
;
                Assert.IsNotNull(Adapters);
            }
        }

        [TestClass]
        public class DNS
        {
            [TestMethod]
            public async Task Set()
            {
                var Result = await Automation.Network.DNS.Set(new[] { IPAddress.Parse("127.0.0.1") });

                Assert.AreEqual(true, Result);
            }

            [TestMethod]
            public async Task Reset()
            {
                var Result =  await Automation.Network.DNS.Reset();

                Assert.AreEqual(true, Result);
            }
        }
    }
}

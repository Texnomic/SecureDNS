using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using Texnomic.ORMi;
using Texnomic.WMI.Network;
using static Texnomic.WMI.Network.NetworkAdapterConfiguration;

namespace Texnomic.SecureDNS.Tests
{
    [TestClass]
    public class AdapterConfiguration
    {
        [TestMethod]
        public void Query()
        {
            var Wmi = new WmiContext();

            var NICs = Wmi.Query<NetworkAdapterConfiguration>();

            Assert.IsTrue(NICs.Count() > 0);
        }

        [TestMethod]
        public void Set()
        {
            var Wmi = new WmiContext();

            var NICs = Wmi.Query<NetworkAdapterConfiguration>();

            var NIC = NICs.Where(Int => Int.Index == 9).First();

            var Result = NIC.SetDnsServers(IPAddress.Parse("127.0.0.1"));

            Assert.AreEqual(Result, SetDnsServersResult.AccessDenied);
        }
    }
}

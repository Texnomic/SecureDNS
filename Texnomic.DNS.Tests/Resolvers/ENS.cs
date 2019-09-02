using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Texnomic.DNS.Tests.Resolvers
{
    [TestClass]
    public class ENS
    {
        private DNS.Resolvers.ENS Resolver;

        [TestInitialize]
        public void Initialize()
        {
            Resolver = new DNS.Resolvers.ENS("7238211010344719ad14a89db874158c");
        }

        [TestMethod]
        public async Task StringAsync()
        {
            var Address = await Resolver.ResolveAsync("nickjohnson.eth");

            Assert.AreEqual("0xfdb33f8ac7ce72d7d4795dd8610e323b4c122fbb", Address);
        }
    }
}

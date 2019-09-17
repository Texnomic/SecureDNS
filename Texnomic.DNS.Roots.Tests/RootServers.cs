using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Roots.Models;

namespace Texnomic.DNS.Roots.Tests
{
    [TestClass]
    public class Roots
    {
        [TestMethod]
        public async Task GetLists()
        {
            var Lists = await RootsClient.GetList();

            Assert.IsNotNull(Lists);
            Assert.IsInstanceOfType(Lists, typeof(Root[]));
        }
    }
}

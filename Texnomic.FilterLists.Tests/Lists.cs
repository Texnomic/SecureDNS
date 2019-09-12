using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Texnomic.FilterLists.Tests
{
    [TestClass]
    public class Lists
    {
        [TestMethod]
        public async Task GetLists()
        {
            var FilterListsClient = new FilterListsClient();

            var Lists = await FilterListsClient.GetLists();

            Assert.IsNotNull(Lists);
        }
    }
}

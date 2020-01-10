using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.FilterLists.Models;

namespace Texnomic.FilterLists.Tests
{
    [TestClass]
    public class FilterLists
    {
        [TestMethod]
        public async Task GetLists()
        {
            var Client = new FilterListsClient();

            var Lists = await Client.GetListsAsync();

            Assert.IsNotNull(Lists);
            Assert.IsInstanceOfType(Lists,typeof(List<FilterList>));
        }
    }
}

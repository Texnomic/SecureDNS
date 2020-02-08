using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Texnomic.DNS.Tests.Stamps
{
    [TestClass]
    public class DNSCrypt
    {
        [TestMethod]
        public async Task DeserializeAsync()
        {
            var StampEncoded = "sdns://AQYAAAAAAAAADTkuOS45LjEwOjg0NDMgZ8hHuMh1jNEgJFVDvnVnRt803x2EwAuMRwNo34Idhj4ZMi5kbnNjcnlwdC1jZXJ0LnF1YWQ5Lm5ldA";

            var Stamp =  await Models.Stamp.FromStringAsync(StampEncoded);

            Assert.IsNotNull(Stamp);
        }

    }
}

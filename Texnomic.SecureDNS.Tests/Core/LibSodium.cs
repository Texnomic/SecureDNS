using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sodium;

namespace Texnomic.SecureDNS.Tests.Core
{
    [TestClass]
    public class LibSodium
    {
        [TestMethod]
        public void Decrypt()
        {
            var Message = Encoding.ASCII.GetBytes("Hello World");

            var Key = SodiumCore.GetRandomBytes(32);

            var Nonce = SodiumCore.GetRandomBytes(24);

            var CipherText = SecretBox.Create(Message, Nonce, Key);

            var PlainText = SecretBox.Open(CipherText, Nonce, Key);

            Assert.AreEqual(Encoding.ASCII.GetString(Message), Encoding.ASCII.GetString(PlainText));

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Tests.Providers.TLS
{
    [TestClass]
    public class Quad9
    {
        private ushort ID;
        private IProtocol Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;


        [TestInitialize]
        public void Initialize()
        {
            ID = (ushort) new Random().Next();

            Resolver = new DNS.Protocols.TLS(IPAddress.Parse("9.9.9.9"), "047D8BD71D03850D1825B3341C29A127D4AC0125488AA0F1EA02B9D8512C086AAC7256ECFA3DA6A09F4909558EACFEB973175C02FB78CC2491946F4323890E1D66");

            RequestMessage = new Message()
            {
                ID = ID,
                RecursionDesired = true,
                Questions = new List<IQuestion>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("google.com"),
                        Class = RecordClass.Internet,
                        Type = RecordType.A
                    }
                }
            };
        }

        [TestMethod]
        public async Task QueryAsync()
        {
            ResponseMessage = await Resolver.ResolveAsync(RequestMessage);

            Assert.AreEqual(ID, ResponseMessage.ID);
            Assert.IsNotNull(ResponseMessage.Questions);
            Assert.IsNotNull(ResponseMessage.Answers);
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.A));
        }
    }
}

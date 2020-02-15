using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Options;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Tests.Records
{
    [TestClass]
    public class RRSIG
    {
        private ushort ID;
        private IProtocol Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;
        

        [TestInitialize]
        public void Initialize()
        {
            ID = (ushort)new Random().Next();

            //Note: RRSIG Only works over TCP/TLS !!

            var TLSOptions = new TLSOptions()
            {
                Host = "8.8.4.4",
                Timeout = 4000,
            };

            var TLSOptionsMonitor = Mock.Of<IOptionsMonitor<TLSOptions>>(Options => Options.CurrentValue == TLSOptions);

            Resolver = new TLS(TLSOptionsMonitor);

            RequestMessage = new Message()
            {
                ID = ID,
                Truncated = false,
                RecursionDesired = true,
                Questions =new List<IQuestion>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("google"),
                        Class = RecordClass.Internet,
                        Type = RecordType.RRSIG
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
            Assert.IsInstanceOfType(ResponseMessage.Answers[0].Record, typeof(DNS.Records.RRSIG));
        }
    }
}

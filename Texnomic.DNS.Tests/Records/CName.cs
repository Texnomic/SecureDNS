using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Tests.Records
{
    [TestClass]
    public class CName
    {
        private ushort ID;
        private IProtocol Resolver;
        private Message RequestMessage;
        private Message ResponseMessage;
        

        [TestInitialize]
        public void Initialize()
        {
            ID = (ushort)new Random().Next();

            Resolver = new UDP(IPAddress.Parse("8.8.8.8"));

            RequestMessage = new Message()
            {
                ID = ID,
                RecursionDesired = true,
                Questions = new List<Question>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("www.texnomic.com"),
                        Class = RecordClass.Internet,
                        Type = RecordType.CNAME
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
            Assert.AreEqual(ResponseCode.NXRRSet, ResponseMessage.ResponseCode);
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.CName));
        }
    }
}

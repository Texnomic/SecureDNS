using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Resolvers;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Tests.Resolvers
{
    [TestClass]
    public class ENS
    {
        private ushort ID;
        private DNS.Resolvers.ENS Resolver;
        private Message RequestMessage;
        private Message ResponseMessage;

        [TestInitialize]
        public void Initialize()
        {
            Resolver = new DNS.Resolvers.ENS("7238211010344719ad14a89db874158c");

            ID = (ushort) new Random().Next();

            RequestMessage = new Message()
            {
                ID = ID,
                RecursionDesired = true,
                Questions = new List<Question>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("nickjohnson.eth"),
                        Class = RecordClass.Internet,
                        Type = RecordType.ETH
                    }
                }
            };
        }

        [TestMethod]
        public async Task StringAsync()
        {
            var Address = await Resolver.ResolveAsync("nickjohnson.eth");

            Assert.AreEqual("0xfdb33f8ac7ce72d7d4795dd8610e323b4c122fbb", Address);
        }


        [TestMethod]
        public async Task MessageAsync()
        {
            ResponseMessage = await Resolver.ResolveAsync(RequestMessage);

            Assertions();
        }

        [TestMethod]
        public async Task BytesAsync()
        {
            var RequestBytes = RequestMessage.ToArray();

            var ResponseBytes = await Resolver.ResolveAsync(RequestBytes);

            ResponseMessage = Message.FromArray(ResponseBytes);

            Assertions();
        }

        public void Assertions()
        {
            Assert.IsNotNull(ResponseMessage);
            Assert.AreEqual(ID, ResponseMessage.ID);
            Assert.IsNotNull(ResponseMessage.Questions);
            Assert.IsNotNull(ResponseMessage.Answers);
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.ETH));
        }
    }
}

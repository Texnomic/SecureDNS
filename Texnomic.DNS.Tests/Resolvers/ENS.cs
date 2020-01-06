using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Tests.Resolvers
{
    [TestClass]
    public class ENS
    {
        private ushort ID;
        private Protocols.ENS Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;
        private BinarySerializer BinarySerializer;

        [TestInitialize]
        public void Initialize()
        {
            BinarySerializer = new BinarySerializer();

            Resolver = new Protocols.ENS(new Uri("https://mainnet.infura.io/v3/7238211010344719ad14a89db874158c"), "0x314159265dd8dbb310642f98f50c066173c1259b");

            ID = (ushort)new Random().Next();

            RequestMessage = new Message()
            {
                ID = ID,
                RecursionDesired = true,
                Questions = new List<IQuestion>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("texnomic.eth"),
                        Class = RecordClass.Internet,
                        Type = RecordType.ETH
                    }
                }
            };
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
            var RequestBytes = await BinarySerializer.SerializeAsync(RequestMessage);

            var ResponseBytes = await Resolver.ResolveAsync(RequestBytes);

            ResponseMessage = await BinarySerializer.DeserializeAsync<Message>(ResponseBytes);

            Assertions();
        }

        private void Assertions()
        {
            Assert.IsNotNull(ResponseMessage);
            Assert.AreEqual(ID, ResponseMessage.ID);
            Assert.IsNotNull(ResponseMessage.Questions);
            Assert.IsNotNull(ResponseMessage.Answers);
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.ETH));
        }
    }
}

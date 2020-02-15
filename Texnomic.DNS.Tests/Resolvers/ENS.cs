using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Options;

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

            var ENSOptions = new ENSOptions();

            var ENSOptionsMonitor = Mock.Of<IOptionsMonitor<ENSOptions>>(Options => Options.CurrentValue == ENSOptions);

            Resolver = new Protocols.ENS(ENSOptionsMonitor);

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
                        Type = RecordType.TXT
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
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.TXT));
        }
    }
}

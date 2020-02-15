using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Options;

namespace Texnomic.DNS.Tests.Providers.HTTPS
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
            ID = (ushort)new Random().Next();

            var TLSOptions = new HTTPsOptions()
            {
                Uri = new Uri($"https://dns.google:443/")
            };

            var TLSOptionsMonitor = Mock.Of<IOptionsMonitor<HTTPsOptions>>(Options => Options.CurrentValue == TLSOptions);

            Resolver = new HTTPs(TLSOptionsMonitor);

            RequestMessage = new Message()
            {
                ID = ID,
                RecursionDesired = true,
                Questions = new List<IQuestion>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("facebook.com"),
                        Class = RecordClass.Internet,
                        Type = RecordType.A
                    }
                }
            };
        }

        [TestMethod]
        public async Task QueryAsync()
        {
            //Using Binary Format Over HTTPs 

            var RequestBytes = await RequestMessage.ToArrayAsync();

            var ResponseBytes = await Resolver.ResolveAsync(RequestBytes);

            ResponseMessage = await Message.FromArrayAsync(ResponseBytes);

            Assert.AreEqual(ID, ResponseMessage.ID);
            Assert.IsNotNull(ResponseMessage.Questions);
            Assert.IsNotNull(ResponseMessage.Answers);
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.A));
        }
    }
}

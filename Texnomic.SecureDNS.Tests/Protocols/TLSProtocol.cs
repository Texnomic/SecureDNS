using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using Texnomic.SecureDNS.Protocols;
using Texnomic.SecureDNS.Protocols.Options;

namespace Texnomic.SecureDNS.Tests.Protocols
{
    [TestClass]
    public class TLSProtocol
    {
        private IProtocol Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;


        [TestInitialize]
        public void Initialize()
        {
            var TLSOptions = new TLSOptions()
            {
                IPv4EndPoint = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 853),
                Timeout = new TimeSpan(0, 0, 0, 10),
                CommonName = "cloudflare-dns.com",
                Thumbprint = "6656840172B4FBBCD6D0A4A103491E93004D195F"
            };

            var TLSOptionsMonitor = Mock.Of<IOptionsMonitor<TLSOptions>>(Options => Options.CurrentValue == TLSOptions);

            Resolver = new TLS(TLSOptionsMonitor);

            RequestMessage = new Message()
            {
                ID = (ushort) new Random().Next(),
                RecursionDesired = true,
                Questions = new List<IQuestion>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("rdweb.wvd.microsoft.com"),
                        Class = RecordClass.Internet,
                        Type = RecordType.A
                    }
                }
            };
        }

        [TestMethod]
        public async Task ResolveAsync()
        {
            ResponseMessage = await Resolver.ResolveAsync(RequestMessage);

            Assert.AreEqual(RequestMessage.ID, ResponseMessage.ID);
            Assert.IsNotNull(ResponseMessage.Questions);
            Assert.IsNotNull(ResponseMessage.Answers);
            Assert.IsInstanceOfType(ResponseMessage.Answers.Last().Record, typeof(SecureDNS.Core.Records.A));
        }
    }
}

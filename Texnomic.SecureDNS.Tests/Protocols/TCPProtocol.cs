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
    public class TCPProtocol
    {
        private IProtocol Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;


        [TestInitialize]
        public void Initialize()
        {
            var TCPOptions = new TCPOptions()
            {
                IPv4Address = "1.1.1.1",
                Port = 53,
                Timeout = new TimeSpan(0, 0, 0, 10)
            };

            var TCPOptionsMonitor = Mock.Of<IOptionsMonitor<TCPOptions>>(Options => Options.CurrentValue == TCPOptions);

            Resolver = new TCP(TCPOptionsMonitor);

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

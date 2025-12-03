using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Moq;

using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using Texnomic.SecureDNS.Protocols;
using Texnomic.SecureDNS.Protocols.Options;

namespace Texnomic.SecureDNS.Tests.Protocols;

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
            IPv4Address = "9.9.9.9",
            Port = 53,
            Timeout = new TimeSpan(0, 0, 0, 10)
        };

        var TCPOptionsMonitor = Mock.Of<IOptionsMonitor<TCPOptions>>(Options => Options.CurrentValue == TCPOptions);

        Resolver = new TCP(TCPOptionsMonitor);

        RequestMessage = new Message()
        {
            ID = (ushort) new Random().Next(),
            RecursionDesired = true
        };
    }

    [TestMethod]
    [TestProperty("rdweb.wvd.microsoft.com", "A")]
    [TestProperty(" mountaineerpublishing.com", "MX")]
    public async Task ResolveAsync()
    {
        var Type = GetType();

        var Method = Type.GetMethod(nameof(ResolveAsync));

        var Attributes = Method?.GetCustomAttributes(typeof(TestPropertyAttribute), false);

        if (Attributes == null) throw new TestCanceledException();

        foreach (var Attribute in Attributes)
        {
            var TestDomain = ((TestPropertyAttribute) Attribute).Name;

            var TestRecord = Enum.Parse<RecordType>(((TestPropertyAttribute) Attribute).Value);

            RequestMessage.Questions = new List<IQuestion>()
            {
                new Question()
                {
                    Domain = Domain.FromString(TestDomain),
                    Class = RecordClass.Internet,
                    Type = TestRecord
                }
            };

            ResponseMessage = await Resolver.ResolveAsync(RequestMessage);

            Assert.AreEqual(RequestMessage.ID, ResponseMessage.ID);
            //Assert.IsNotNull(ResponseMessage.Questions);
            //Assert.IsNotNull(ResponseMessage.Answers);
            //Assert.IsInstanceOfType(ResponseMessage.Answers.Last().Record, typeof(SecureDNS.Core.Records.A));
        }
    }
}
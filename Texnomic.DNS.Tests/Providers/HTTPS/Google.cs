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

namespace Texnomic.DNS.Tests.Providers.HTTPS
{
    [TestClass]
    public class Google
    {
        private ushort ID;
        private IProtocol Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;


        [TestInitialize]
        public void Initialize()
        {
            ID = (ushort) new Random().Next();

            Resolver = new HTTPs(new Uri("https://dns.google"), "3082010A0282010100D080C7CFD780BC2E170564D6BA5D009C01BBB22F2E1C09AD9F9FA8F5190D6D2820194A8D1DA4D06F91D3DE7D3A10257F054DED8D565B48A5EAE9A223184562F40B6955A193D0C7AB276EB848A8A01F80AD09E1A4AE2DD2B8312848E89EBAAA68506372C212E426CF9C1BC9A6CEF2D32561960BA94BF9237C826415312D5CE401D74B33852813ED0AD574177D490681C3114AE8F839D3513FD6FFCF7F1A3FE9758E1F40B38EB3E6539087A862850DF1A740E4E4275AB5639FE28FE614FF013480F9A35BD27103CABC5AA8FD1192F5493DE95528EB6EC5FC1ECC691E1D6CCF622AF63BFD2C77D8A3C9EA2227D9EC70E75D69C317E5E2C27B2386239B8917B44EA90203010001");

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

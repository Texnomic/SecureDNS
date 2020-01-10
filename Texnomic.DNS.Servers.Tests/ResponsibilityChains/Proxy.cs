using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Servers.Middlewares;
using Texnomic.DNS.Servers.ResponsibilityChain;

namespace Texnomic.DNS.Servers.Tests.ResponsibilityChains
{
    [TestClass]
    public class Proxy
    {
        private ushort ID;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;


        [TestInitialize]
        public void Initialize()
        {
            ID = (ushort) new Random().Next();

            RequestMessage = new Message()
            {
                ID = ID,
                RecursionDesired = true,
                Questions = new List<IQuestion>()
                {
                    new Question()
                    {
                        Domain = Domain.FromString("google.com"),
                        Class = RecordClass.Internet,
                        Type = RecordType.A
                    }
                }
            };
        }

        [TestMethod]
        public async Task RunAsync()
        {
            var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
            var Middlewares = new List<Type>()
            {
                typeof(GoogleHTTPsMiddleware),
            };
            var ProxyResponsibilityChain = new ProxyResponsibilityChain(Middlewares, ActivatorMiddlewareResolver);
            ResponseMessage = await ProxyResponsibilityChain.Execute(RequestMessage);

            Assert.AreEqual(ID, ResponseMessage.ID);
            Assert.IsNotNull(ResponseMessage.Questions);
            Assert.IsNotNull(ResponseMessage.Answers);
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(Records.A));
        }
    }
}

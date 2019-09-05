using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Tests.Records
{
    [TestClass]
    public class AAAA
    {
        private ushort ID;
        private IResolver Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;
        

        [TestInitialize]
        public void Initialize()
        {
            ID = (ushort)new Random().Next();

            Resolver = new UDP(IPAddress.Parse("8.8.4.4"));

            RequestMessage = new Message()
            {
                ID = ID,
                //MessageType = MessageType.Query,
                //OperationCode = OperationCode.Query,
                //AuthoritativeAnswer = AuthoritativeAnswer.Cache,
                //Truncated = false,
                RecursionDesired = true,
                //RecursionAvailable = false,
                //Zero = 0,
                //AuthenticatedData = false,
                //CheckingDisabled = false,
                //ResponseCode = ResponseCode.NoError,
                //QuestionsCount = 1,
                //AnswersCount = 0,
                Questions = new[]
                {
                    new Question()
                    {
                        Domain = Domain.FromString("facebook.com"),
                        Class = RecordClass.Internet,
                        Type = RecordType.AAAA
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
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.AAAA));
        }
    }
}

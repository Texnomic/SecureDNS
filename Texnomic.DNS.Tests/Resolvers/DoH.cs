using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Tests.Resolvers
{
    [TestClass]
    public class DoH
    {
        private ushort ID;
        private IResolver Resolver;
        private Message RequestMessage;
        private Message ResponseMessage;


        [TestInitialize]
        public void Initialize()
        {
            Resolver = new HTTPs(IPAddress.Parse("8.8.8.8"),
                "04C520708C204250281E7D44417C3079291C635E1D449BC5F7713A2BDED2A2A4B16C3D6AC877B8CB8F2E5053FDF418267F6137EDFFC2BEE90B5DB97EE1DF1CE274");


            ID = (ushort)new Random().Next();

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
                        Type = RecordType.A
                    }
                }
            };
        }


        [TestMethod]
        public async Task MessageAsync()
        {
            var Msg = await Resolver.ResolveAsync(RequestMessage);

            //Assert.AreEqual(Msg.ID, 39298);
            Assert.AreEqual(Msg.AnswersCount, 1);
            Assert.AreEqual(Msg.QuestionsCount, 1);
            Assert.AreEqual(Msg.AuthorityCount, 0);
            Assert.AreEqual(Msg.AdditionalCount, 0);
            Assert.AreEqual(Msg.Truncated, false);
            Assert.AreEqual(Msg.RecursionAvailable, true);
            Assert.AreEqual(Msg.RecursionDesired, Enums.RecursionDesired.Recursive);
            Assert.AreEqual(Msg.AuthoritativeAnswer, Enums.AuthoritativeAnswer.Cache);
            Assert.AreEqual(Msg.ResponseCode, Enums.ResponseCode.NoError);
            Assert.AreEqual(Msg.OperationCode, Enums.OperationCode.Query);

            var Answer = Msg.Answers?.First();

            Assert.IsNotNull(Answer);
            Assert.AreEqual(Answer.Type, Enums.RecordType.A);
            Assert.AreEqual(Answer.Class, Enums.RecordClass.Internet);
            Assert.AreEqual(Answer.Domain.ToString(), "facebook.com");
        }

        [TestMethod]
        public async Task BytesAsync()
        {
            var Bytes = await Resolver.ResolveAsync(RequestMessage.ToArray());

            var Msg = Message.FromArray(Bytes);

            //Assert.AreEqual(Msg.ID, 39298);
            Assert.AreEqual(Msg.AnswersCount, 1);
            Assert.AreEqual(Msg.QuestionsCount, 1);
            Assert.AreEqual(Msg.AuthorityCount, 0);
            Assert.AreEqual(Msg.AdditionalCount, 0);
            Assert.AreEqual(Msg.Truncated, false);
            Assert.AreEqual(Msg.RecursionAvailable, false);
            Assert.AreEqual(Msg.RecursionDesired, true);
            Assert.AreEqual(Msg.AuthoritativeAnswer, Enums.AuthoritativeAnswer.Cache);
            //Assert.AreEqual(Msg.ResponseCode, Enums.ResponseCode.NoError);
            Assert.AreEqual(Msg.OperationCode, Enums.OperationCode.Query);

            var Answer = Msg.Answers?.First();

            Assert.IsNotNull(Answer);
            Assert.AreEqual(Answer.Type, Enums.RecordType.A);
            Assert.AreEqual(Answer.Class, Enums.RecordClass.Internet);
            Assert.AreEqual(Answer.Domain.ToString(), "facebook.com");
        }
    }
}

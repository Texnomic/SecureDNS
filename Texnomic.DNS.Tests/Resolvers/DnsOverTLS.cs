using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Tests.Resolvers
{
    [TestClass]
    public class DnsOverTLS
    {
        byte[] RequestBytes;
        IResolver Resolver;

        [TestInitialize]
        public void Initialize()
        {
            //{ Header ={ Id = 39298, QuestionCount = 1, AnswerRecordCount = 0, AuthorityRecordCount = 0, AdditionalRecordCount = 0, Response = False, OperationCode = Query, AuthorativeServer = False, Truncated = False, RecursionDesired = True, RecursionAvailable = False, AuthenticData = False, CheckingDisabled = False, ResponseCode = NoError}, Questions =[{ Name = facebook.com, Type = A, Class = IN}], AdditionalRecords =[]}
            RequestBytes = Convert.FromBase64String("AB6ZggEAAAEAAAAAAAAIZmFjZWJvb2sDY29tAAABAAE=");

            Resolver = new DoT(IPAddress.Parse("1.1.1.1"), "04C520708C204250281E7D44417C3079291C635E1D449BC5F7713A2BDED2A2A4B16C3D6AC877B8CB8F2E5053FDF418267F6137EDFFC2BEE90B5DB97EE1DF1CE274");
        }

        [TestMethod]
        public async Task MessageAsync()
        {
            var Msg = Message.FromArray(RequestBytes);

            Msg = await Resolver.ResolveAsync(Msg);

            Assert.AreEqual(Msg.ID, 39298);
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
           var Bytes = await Resolver.ResolveAsync(RequestBytes);

            var Msg = Message.FromArray(Bytes);

            Assert.AreEqual(Msg.ID, 39298);
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
    }
}

using System;
using Texnomic.DNS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BinarySerialization;

namespace Texnomic.DNS.Tests
{
    [TestClass]
    public class Serialization
    {
        readonly byte[] RequestBytes;
        readonly byte[] ResponseBytes;
        readonly BinarySerializer BinarySerializer;

        public Serialization()
        {
            //{ Header ={ Id = 39298, QuestionCount = 1, AnswerRecordCount = 0, AuthorityRecordCount = 0, AdditionalRecordCount = 0, Response = False, OperationCode = Query, AuthorativeServer = False, Truncated = False, RecursionDesired = True, RecursionAvailable = False, AuthenticData = False, CheckingDisabled = False, ResponseCode = NoError}, Questions =[{ Name = facebook.com, Type = A, Class = IN}], AdditionalRecords =[]}
            RequestBytes = Convert.FromBase64String("AB6ZggEAAAEAAAAAAAAIZmFjZWJvb2sDY29tAAABAAE=");

            //{ Header ={ Id = 39298, QuestionCount = 1, AnswerRecordCount = 0, AuthorityRecordCount = 0, AdditionalRecordCount = 0, Response = False, OperationCode = Query, AuthorativeServer = False, Truncated = False, RecursionDesired = True, RecursionAvailable = False, AuthenticData = False, CheckingDisabled = False, ResponseCode = NoError}, Questions =[{ Name = facebook.com, Type = A, Class = IN}], AdditionalRecords =[]} => { Header ={ Id = 39298, QuestionCount = 1, AnswerRecordCount = 1, AuthorityRecordCount = 0, AdditionalRecordCount = 0, Response = True, OperationCode = Query, AuthorativeServer = False, Truncated = False, RecursionDesired = True, RecursionAvailable = True, AuthenticData = False, CheckingDisabled = False, ResponseCode = NoError}, Questions =[{ Name = facebook.com, Type = A, Class = IN}], AnswerRecords =[{ Name = facebook.com, Type = A, Class = IN, TimeToLive = 00:03:10, DataLength = 4, IPAddress = 179.60.192.36}], AuthorityRecords =[], AdditionalRecords =[]}
            ResponseBytes = Convert.FromBase64String("AC6ZgoGAAAEAAQAAAAAIZmFjZWJvb2sDY29tAAABAAHADAABAAEAAAEeAASzPMAk");

            BinarySerializer = new BinarySerializer();
        }

        [TestMethod]
        public void FromArray()
        {
            var Msg = BinarySerializer.Deserialize<Message>(ResponseBytes);


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

            var Question = Msg.Questions?.First();

            Assert.IsNotNull(Question);
            Assert.AreEqual(Question.Type, Enums.RecordType.A);
            Assert.AreEqual(Question.Class, Enums.RecordClass.Internet);
            Assert.AreEqual(Question.Domain.ToString(), "facebook.com");

            var Answer = Msg.Answers?.First();

            Assert.IsNotNull(Answer);
            Assert.AreEqual(Answer.Type, Enums.RecordType.A);
            Assert.AreEqual(Answer.Class, Enums.RecordClass.Internet);
        }

        [TestMethod]
        public void ToArray()
        {
            var Msg = Message.FromArray(ResponseBytes);

            var Bytes = Msg.ToArray();

            Assert.AreEqual(Convert.ToBase64String(ResponseBytes), Convert.ToBase64String(Bytes));
        }
    }
}

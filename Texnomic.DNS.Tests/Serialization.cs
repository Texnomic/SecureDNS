using System;
using Texnomic.DNS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Texnomic.DNS.Tests
{
    [TestClass]
    public class Serialization
    {
        [TestMethod]
        public void Request()
        {
            //{ Header ={ Id = 39298, QuestionCount = 1, AnswerRecordCount = 0, AuthorityRecordCount = 0, AdditionalRecordCount = 0, Response = False, OperationCode = Query, AuthorativeServer = False, Truncated = False, RecursionDesired = True, RecursionAvailable = False, AuthenticData = False, CheckingDisabled = False, ResponseCode = NoError}, Questions =[{ Name = facebook.com, Type = A, Class = IN}], AdditionalRecords =[]}
            var RequestBytes = Convert.FromBase64String("mYIBAAABAAAAAAAACGZhY2Vib29rA2NvbQAAAQAB");

            var Msg = Message.FromArray(RequestBytes);

            Assert.AreEqual(Msg.QuestionsCount, 1);
        }

        [TestMethod]
        public void Response()
        {
            //{ Header ={ Id = 39298, QuestionCount = 1, AnswerRecordCount = 0, AuthorityRecordCount = 0, AdditionalRecordCount = 0, Response = False, OperationCode = Query, AuthorativeServer = False, Truncated = False, RecursionDesired = True, RecursionAvailable = False, AuthenticData = False, CheckingDisabled = False, ResponseCode = NoError}, Questions =[{ Name = facebook.com, Type = A, Class = IN}], AdditionalRecords =[]} => { Header ={ Id = 39298, QuestionCount = 1, AnswerRecordCount = 1, AuthorityRecordCount = 0, AdditionalRecordCount = 0, Response = True, OperationCode = Query, AuthorativeServer = False, Truncated = False, RecursionDesired = True, RecursionAvailable = True, AuthenticData = False, CheckingDisabled = False, ResponseCode = NoError}, Questions =[{ Name = facebook.com, Type = A, Class = IN}], AnswerRecords =[{ Name = facebook.com, Type = A, Class = IN, TimeToLive = 00:03:10, DataLength = 4, IPAddress = 179.60.192.36}], AuthorityRecords =[], AdditionalRecords =[]}
            var ResponseBytes = Convert.FromBase64String("mYKBgAABAAEAAAAACGZhY2Vib29rA2NvbQAAAQABCGZhY2Vib29rA2NvbQAAAQABAAAAvgAEszzAJA==");

            var Msg = Message.FromArray(ResponseBytes);

            Assert.AreEqual(Msg.QuestionsCount, 1);

            Assert.AreEqual(Msg.AnswersCount, 1);
        }
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Records;

namespace Texnomic.DNS.Tests.Serialization.Binary
{
    [TestClass]
    public class Response
    {
        /// <summary>
        /// WireShark => Show Packet Bytes => Decode As None => Show As C Array
        /// </summary>
        private byte[] RequestBytes =
        {
            0x9b, 0xf2, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x08, 0x66, 0x61, 0x63,
            0x65, 0x62, 0x6f, 0x6f, 0x6b, 0x03, 0x63, 0x6f,
            0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c,
            0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x2b,
            0x00, 0x04, 0xb3, 0x3c, 0xc0, 0x24
        };

        /// <summary>
        /// Prepend Array with Big Indian Message Length
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var Length = BitConverter.GetBytes((ushort)RequestBytes.Length);
            Array.Reverse(Length);
            var Bytes = new byte[RequestBytes.Length + 2];
            Array.Copy(Length, Bytes, 2);
            Array.Copy(RequestBytes, 0, Bytes, 2, RequestBytes.Length);
            RequestBytes = Bytes;
        }

        [TestMethod, Priority(1)]
        public void FromArray()
        {
            var Msg = Message.FromArray(RequestBytes);
            //Assert.AreEqual(Msg.ID, 256);
            Assert.AreEqual(MessageType.Response, Msg.MessageType);
            Assert.AreEqual(OperationCode.Query, Msg.OperationCode);
            Assert.AreEqual(AuthoritativeAnswer.Cache, Msg.AuthoritativeAnswer);
            Assert.AreEqual(false, Msg.Truncated);
            Assert.AreEqual(RecursionDesired.Recursive, Msg.RecursionDesired);
            //Assert.AreEqual(true, Msg.RecursionAvailable);
            Assert.AreEqual(0, Msg.Zero);
            Assert.AreEqual(false, Msg.AuthenticatedData);
            Assert.AreEqual(false, Msg.CheckingDisabled);
            //Assert.AreEqual(ResponseCode.NoError, Msg.ResponseCode);
            Assert.AreEqual(1, Msg.QuestionsCount);
            Assert.AreEqual(1, Msg.AnswersCount);
            Assert.AreEqual(0, Msg.AuthorityCount);
            Assert.AreEqual(0, Msg.AdditionalCount);
            

            Assert.IsNotNull(Msg.Questions);
            var Question = Msg.Questions?.First();
            Assert.IsNotNull(Question);
            Assert.AreEqual(RecordType.A, Question.Type);
            Assert.AreEqual(RecordClass.Internet, Question.Class);
            Assert.AreEqual("facebook.com", Question.Domain.Name);


            Assert.IsNotNull(Msg.Answers);
            var Answer = Msg.Answers?.First();
            Assert.IsNotNull(Question);
            Assert.AreEqual(RecordType.A, Answer.Type);
            Assert.AreEqual(RecordClass.Internet, Answer.Class);
            Assert.AreEqual("facebook.com", Answer.Domain.Name);
            Assert.IsInstanceOfType(Answer.Record, typeof(A));
        }

        [TestMethod, Priority(2)]
        public void ToArray()
        {
            var Msg = Message.FromArray(RequestBytes);

            var Bytes = Msg.ToArray();

            Assert.AreEqual(Convert.ToBase64String(RequestBytes), Convert.ToBase64String(Bytes));
        }
    }
}

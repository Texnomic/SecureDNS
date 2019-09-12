using System;
using System.Linq;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Tests.Serialization.Binary
{
    [TestClass]
    public class Request
    {
        private readonly BinarySerializer BinarySerializer = new BinarySerializer();

        /// <summary>
        /// WireShark => Show Packet Bytes => Decode As None => Show As C Array
        /// </summary>
        private byte[] RequestBytes =
        {
            0x9b, 0xf2, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x08, 0x66, 0x61, 0x63,
            0x65, 0x62, 0x6f, 0x6f, 0x6b, 0x03, 0x63, 0x6f,
            0x6d, 0x00, 0x00, 0x01, 0x00, 0x01
        };

        [TestMethod, Priority(1)]
        public async Task FromArray()
        {
            var Message = await BinarySerializer.DeserializeAsync<Message>(RequestBytes);

            Assert.AreEqual(Message.ID, 39922);
            Assert.AreEqual(MessageType.Query, Message.MessageType);
            Assert.AreEqual(OperationCode.Query, Message.OperationCode);
            Assert.AreEqual(AuthoritativeAnswer.Cache, Message.AuthoritativeAnswer);
            Assert.AreEqual(false, Message.Truncated);
            Assert.AreEqual(true, Message.RecursionDesired);
            Assert.AreEqual(false, Message.RecursionAvailable);
            Assert.AreEqual(0, Message.Zero);
            Assert.AreEqual(false, Message.AuthenticatedData);
            Assert.AreEqual(false, Message.CheckingDisabled);
            Assert.AreEqual(ResponseCode.NoError, Message.ResponseCode);
            Assert.AreEqual(1, Message.QuestionsCount);
            Assert.AreEqual(0, Message.AnswersCount);
            Assert.AreEqual(0, Message.AuthorityCount);
            Assert.AreEqual(0, Message.AdditionalCount);


            Assert.IsNotNull(Message.Questions);
            var Question = Message.Questions?.First();
            Assert.IsNotNull(Question);
            Assert.AreEqual(RecordType.A, Question.Type);
            Assert.AreEqual(RecordClass.Internet, Question.Class);
            Assert.AreEqual("facebook.com", Question.Domain.Name);
        }

        [TestMethod, Priority(2)]
        public async Task ToArray()
        {
            var Message = await BinarySerializer.DeserializeAsync<Message>(RequestBytes);

            var Bytes = await BinarySerializer.SerializeAsync(Message);

            Assert.AreEqual(Convert.ToBase64String(RequestBytes), Convert.ToBase64String(Bytes));
        }
    }
}

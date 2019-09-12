using System;
using System.Linq;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Records;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Tests.Serialization.Binary
{
    [TestClass]
    public class Response
    {
        private readonly BinarySerializer BinarySerializer = new BinarySerializer();

        /// <summary>
        /// WireShark => Show Packet Bytes => Decode As None => Show As C Array
        /// </summary>
        private readonly byte[] ResponseBytes =
        {
            0x9b, 0xf2, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x08, 0x66, 0x61, 0x63,
            0x65, 0x62, 0x6f, 0x6f, 0x6b, 0x03, 0x63, 0x6f,
            0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c,
            0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x2b,
            0x00, 0x04, 0xb3, 0x3c, 0xc0, 0x24
        };

        [TestMethod, Priority(1)]
        public async Task FromArray()
        {
            var Msg = await BinarySerializer.DeserializeAsync<Message>(ResponseBytes);

            Assert.AreEqual(Msg.ID, 39922);
            Assert.AreEqual(MessageType.Response, Msg.MessageType);
            Assert.AreEqual(OperationCode.Query, Msg.OperationCode);
            Assert.AreEqual(AuthoritativeAnswer.Cache, Msg.AuthoritativeAnswer);
            Assert.AreEqual(false, Msg.Truncated);
            Assert.AreEqual(true, Msg.RecursionDesired);
            Assert.AreEqual(true, Msg.RecursionAvailable);
            Assert.AreEqual(0, Msg.Zero);
            Assert.AreEqual(false, Msg.AuthenticatedData);
            Assert.AreEqual(false, Msg.CheckingDisabled);
            Assert.AreEqual(ResponseCode.NoError, Msg.ResponseCode);
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
        public async Task ToArray()
        {
            var Message = await BinarySerializer.DeserializeAsync<Message>(ResponseBytes);

            var Bytes = await BinarySerializer.SerializeAsync(Message);

            var OriginalResponse = PrintBinary(ResponseBytes);
            var SerializedResponse = PrintBinary(Bytes);

            //Will never match unless compressed labels are supported in Serialization Phase. 

            Assert.AreEqual(Convert.ToBase64String(ResponseBytes), Convert.ToBase64String(Bytes));
        }

        public string PrintBinary(byte[] Bytes)
        {
            return string.Join(' ', Bytes.Select(Byte => Convert.ToString(Byte, 2).PadLeft(8, '0')));
        }
    }
}

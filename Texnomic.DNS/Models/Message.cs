using BinarySerialization;
using Nerdbank.Streams;
using System.Buffers;
using System.IO;
using System.Text.Json;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Message
    {
        [Ignore]
        private static readonly BinarySerializer Serializer = new BinarySerializer();

        [FieldOrder(0)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public ushort Length { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public ushort ID { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(1)]
        public MessageType MessageType { get; set; }

        [FieldOrder(3)]
        [FieldBitLength(4)]
        public OperationCode OperationCode { get; set; }

        [FieldOrder(4)]
        [FieldBitLength(1)]
        public AuthoritativeAnswer AuthoritativeAnswer { get; set; }

        [FieldOrder(5)]
        [FieldBitLength(1)]
        public bool Truncated { get; set; }

        [FieldOrder(6)]
        [FieldBitLength(1)]
        public RecursionDesired RecursionDesired { get; set; }

        [FieldOrder(7)]
        [FieldBitLength(1)]
        public bool RecursionAvailable { get; set; }

        [FieldOrder(8)]
        [FieldBitLength(1)]
        public int Zero { get; set; }

        [FieldOrder(9)]
        [FieldBitLength(1)]
        public bool AuthenticatedData { get; set; }

        [FieldOrder(10)]
        [FieldBitLength(1)]
        public bool CheckingDisabled { get; set; }

        [FieldOrder(11)]
        [FieldBitLength(4)]
        public ResponseCode ResponseCode { get; set; }

        [FieldOrder(12)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public ushort QuestionsCount { get; set; }

        [FieldOrder(13)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public ushort AnswersCount { get; set; }

        [FieldOrder(14)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public ushort AuthorityCount { get; set; }

        [FieldOrder(15)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public ushort AdditionalCount { get; set; }

        [FieldOrder(16)]
        [FieldCount(nameof(QuestionsCount))]
        public Question[] Questions { get; set; }

        [FieldOrder(17)]
        [FieldCount(nameof(AnswersCount))]
        public Answer[] Answers { get; set; }

        public byte[] ToArray()
        {
            using var Stream = new MemoryStream();
            Serializer.Serialize(Stream, this);
            return Stream.ToArray();
        }

        public static Message FromArray(byte[] Data)
        {
            return Serializer.Deserialize<Message>(Data);
        }

        public static Message FromArray(ReadOnlySequence<byte> Data)
        {
            return Serializer.Deserialize<Message>(Data.AsStream());
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message FromJson(string Json)
        {
            return JsonSerializer.Deserialize<Message>(Json);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}

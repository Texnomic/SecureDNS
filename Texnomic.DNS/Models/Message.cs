using BinarySerialization;
using Nerdbank.Streams;
using System.Buffers;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Texnomic.DNS.Converters;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Message
    {
        [FieldOrder(0)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort Length { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort ID { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(1)]
        [JsonIgnore]
        public MessageType MessageType { get; set; }

        [FieldOrder(3)]
        [FieldBitLength(4)]
        [JsonIgnore]
        public OperationCode OperationCode { get; set; }

        [FieldOrder(4)]
        [FieldBitLength(1)]
        [JsonIgnore]
        public AuthoritativeAnswer AuthoritativeAnswer { get; set; }

        [FieldOrder(5)]
        [FieldBitLength(1)]
        [JsonPropertyName("TC")]
        public bool Truncated { get; set; }

        [FieldOrder(6)]
        [FieldBitLength(1)]
        [JsonPropertyName("RD")]
        public RecursionDesired RecursionDesired { get; set; }

        [FieldOrder(7)]
        [FieldBitLength(1)]
        [JsonPropertyName("RA")]
        public bool RecursionAvailable { get; set; }

        [FieldOrder(8)]
        [FieldBitLength(1)]
        [JsonIgnore]
        public int Zero { get; set; }

        [FieldOrder(9)]
        [FieldBitLength(1)]
        [JsonPropertyName("AD")]
        public bool AuthenticatedData { get; set; }

        [FieldOrder(10)]
        [FieldBitLength(1)]
        [JsonPropertyName("CD")]
        public bool CheckingDisabled { get; set; }

        [FieldOrder(11)]
        [FieldBitLength(4)]
        [JsonPropertyName("Status")]
        public ResponseCode ResponseCode { get; set; }

        [FieldOrder(12)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort QuestionsCount { get; set; }

        [FieldOrder(13)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort AnswersCount { get; set; }

        [FieldOrder(14)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort AuthorityCount { get; set; }

        [FieldOrder(15)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort AdditionalCount { get; set; }

        [FieldOrder(16)]
        [FieldCount(nameof(QuestionsCount))]
        [JsonPropertyName("Question")]
        public Question[] Questions { get; set; }

        [FieldOrder(17)]
        [FieldCount(nameof(AnswersCount))]
        [JsonPropertyName("Answer")]
        public Answer[] Answers { get; set; }

        [Ignore]
        [JsonPropertyName("Comment")]
        public string Comment { get; set; }

        public byte[] ToArray()
        {
            var Serializer = new BinarySerializer();
            using var Stream = new MemoryStream();
            Serializer.Serialize(Stream, this);
            return Stream.ToArray();
        }

        public static Message FromArray(byte[] Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Message>(Data);
        }

        public static Message FromArray(ReadOnlySequence<byte> Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Message>(Data.AsStream());
        }

        public string ToJson()
        {
            var JsonSerializerOptions = new JsonSerializerOptions();

            JsonSerializerOptions.Converters.Add(new RecursionDesiredConverter());

            return JsonSerializer.Serialize(this, JsonSerializerOptions);
        }

        public static Message FromJson(string Json)
        {
            var JsonSerializerOptions = new JsonSerializerOptions();

            JsonSerializerOptions.Converters.Add(new RecursionDesiredConverter());

            return JsonSerializer.Deserialize<Message>(Json, JsonSerializerOptions);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}

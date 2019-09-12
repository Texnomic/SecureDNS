using BinarySerialization;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Models
{
    public class Message : IMessage
    {
        [FieldOrder(1), FieldBitLength(16), FieldEndianness(Endianness.Big), JsonIgnore]
        public ushort ID { get; set; }

        [FieldOrder(6), FieldBitLength(1), JsonIgnore]
        public MessageType MessageType { get; set; }

        [FieldOrder(5), FieldBitLength(4), JsonIgnore]
        public OperationCode OperationCode { get; set; }

        [FieldOrder(4), FieldBitLength(1), JsonIgnore]
        public AuthoritativeAnswer AuthoritativeAnswer { get; set; }

        [FieldOrder(3), FieldBitLength(1), JsonPropertyName("TC")]
        public bool Truncated { get; set; }

        [FieldOrder(2), FieldBitLength(1), FieldEndianness(Endianness.Big), JsonPropertyName("RD")]
        public bool RecursionDesired { get; set; }

        [FieldOrder(11), FieldBitLength(1), JsonPropertyName("RA")]
        public bool RecursionAvailable { get; set; }

        [FieldOrder(10), FieldBitLength(1), JsonIgnore]
        public int Zero { get; set; } = 0;

        [FieldOrder(9), FieldBitLength(1), JsonPropertyName("AD")]
        public bool AuthenticatedData { get; set; }

        [FieldOrder(8), FieldBitLength(1), JsonPropertyName("CD")]
        public bool CheckingDisabled { get; set; }

        [FieldOrder(7), FieldBitLength(4), FieldEndianness(Endianness.Big), JsonPropertyName("Status")]
        public ResponseCode ResponseCode { get; set; }

        private ushort? _QuestionsCount;

        [FieldOrder(12), FieldBitLength(16), FieldEndianness(Endianness.Big), JsonIgnore]
        public ushort QuestionsCount
        {
            get => _QuestionsCount ?? (ushort)(Questions?.Count ?? 0);
            set => _QuestionsCount = value;
        }

        private ushort? _AnswersCount;

        [FieldOrder(13), FieldBitLength(16), FieldEndianness(Endianness.Big), JsonIgnore]
        public ushort AnswersCount
        {
            get => _AnswersCount ?? (ushort)(Answers?.Count ?? 0);
            set => _AnswersCount = value;
        }

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
        [ItemSubtypeDefault(typeof(Question))]
        [JsonPropertyName("Question")]
        public List<IQuestion> Questions { get; set; }

        [FieldOrder(17)]
        [FieldCount(nameof(AnswersCount))]
        [ItemSubtypeDefault(typeof(Answer))]
        [JsonPropertyName("Answer")]
        public List<IAnswer> Answers { get; set; }

        [FieldOrder(18)]
        [FieldCount(nameof(AuthorityCount))]
        [ItemSubtypeDefault(typeof(Answer))]
        [JsonPropertyName("Authority")]
        public List<IAnswer> Authority { get; set; }

        [Ignore]
        [JsonPropertyName("Comment")]
        public string Comment { get; set; }

        public string ToJson()
        {
            var JsonSerializerOptions = new JsonSerializerOptions();
            return JsonSerializer.Serialize(this, JsonSerializerOptions);
        }

        public override string ToString()
        {
            return ToJson();
        }

        public static Message FromArray(byte[] Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Message>(Data);
        }

        public static Task<Message> FromArrayAsync(byte[] Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.DeserializeAsync<Message>(Data);
        }

        public byte[] ToArray()
        {
            var Serializer = new BinarySerializer();
            return Serializer.Serialize(this);
        }

        public async Task<byte[]> ToArrayAsync()
        {
            var Serializer = new BinarySerializer();
            return await Serializer.SerializeAsync(this);
        }
    }
}

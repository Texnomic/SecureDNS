using System;
using BinarySerialization;
using Nerdbank.Streams;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Texnomic.DNS.Enums;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Models
{
    public class Message
    {
        //private ushort? Size;

        //[FieldOrder(0)]
        //[FieldBitLength(16)]
        //[FieldEndianness(Endianness.Big)]
        //[JsonIgnore]
        //public ushort Length
        //{
        //    get => Size ?? CalculateLength();
        //    set => Size = value;
        //}

        [FieldOrder(1)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort ID { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(1)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public MessageType MessageType { get; set; }

        [FieldOrder(3)]
        [FieldBitLength(4)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public OperationCode OperationCode { get; set; }

        [FieldOrder(4)]
        [FieldBitLength(1)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public AuthoritativeAnswer AuthoritativeAnswer { get; set; }

        [FieldOrder(5), FieldBitLength(1), FieldEndianness(Endianness.Big), JsonPropertyName("TC")]
        public bool Truncated { get; set; }

        //BUG When True, Entire Byte Is flipped to entire message became response
        [FieldOrder(6), FieldBitLength(1), FieldEndianness(Endianness.Big), JsonPropertyName("RD")]
        public bool RecursionDesired { get; set; }

        [FieldOrder(7)]
        [FieldBitLength(1)]
        [JsonPropertyName("RA")]
        public bool RecursionAvailable { get; set; }

        [FieldOrder(8), FieldBitLength(1), JsonIgnore]
        public int Zero { get; set; } = 0;

        [FieldOrder(9), FieldBitLength(1), JsonPropertyName("AD")]
        public bool AuthenticatedData { get; set; }

        [FieldOrder(10), FieldBitLength(1), JsonPropertyName("CD")]
        public bool CheckingDisabled { get; set; }

        [FieldOrder(11), FieldBitLength(4), FieldEndianness(Endianness.Big), JsonPropertyName("Status")]
        public ResponseCode ResponseCode { get; set; }

        private ushort? _QuestionsCount;

        [FieldOrder(12), FieldBitLength(16), FieldEndianness(Endianness.Big), JsonIgnore]
        public ushort QuestionsCount
        {
            get => _QuestionsCount ?? (ushort)(Questions?.Length ?? 0);
            set => _QuestionsCount = value;
        }

        private ushort? _AnswersCount;

        [FieldOrder(13), FieldBitLength(16), FieldEndianness(Endianness.Big), JsonIgnore]
        public ushort AnswersCount
        {
            get => _AnswersCount ?? (ushort) (Answers?.Length ?? 0);
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
        [JsonPropertyName("Question")]
        public Question[] Questions { get; set; }

        [FieldOrder(17)]
        [FieldCount(nameof(AnswersCount))]
        [JsonPropertyName("Answer")]
        public Answer[] Answers { get; set; }

        [Ignore]
        [JsonPropertyName("Comment")]
        public string Comment { get; set; }

        //private ushort CalculateLength()
        //{
        //    var Serializer = new BinarySerializer();
        //    return (ushort)Serializer.SizeOf(this);
        //}

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
            return JsonSerializer.Serialize(this, JsonSerializerOptions);
        }

        public async Task<string> ToJsonAsync()
        {
            var JsonSerializerOptions = new JsonSerializerOptions();
            await using var Stream = new MemoryStream();
            using var Reader = new StreamReader(Stream);
            await JsonSerializer.SerializeAsync(Stream, this, JsonSerializerOptions);
            return await Reader.ReadToEndAsync();
        }

        public static Message FromJson(string Json)
        {
            var JsonSerializerOptions = new JsonSerializerOptions();
            return JsonSerializer.Deserialize<Message>(Json, JsonSerializerOptions);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}

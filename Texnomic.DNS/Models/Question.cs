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
    public class Question
    {
        [FieldOrder(0)]
        [JsonIgnore]
        public Domain Domain { get; set; }

        [Ignore]
        [JsonPropertyName("name")]
        public string Name
        {
            get => Domain.Text;
            set => Domain = Domain.FromString(value);
        }

        [FieldOrder(1)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonPropertyName("type")]
        public RecordType Type { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public RecordClass Class { get; set; }

        public byte[] ToArray()
        {
            var Serializer = new BinarySerializer();
            using var Stream = new MemoryStream();
            Serializer.Serialize(Stream, this);
            return Stream.ToArray();
        }

        public static Question FromArray(byte[] Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Question>(Data);
        }

        public static Question FromArray(ReadOnlySequence<byte> Data)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Question>(Data.AsStream());
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

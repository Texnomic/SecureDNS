using BinarySerialization;
using System.Text.Json.Serialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Factories;

namespace Texnomic.DNS.Models
{
    public class Answer : Question
    {
        [FieldOrder(3)]
        [JsonPropertyName("TTL")]
        public TimeToLive TimeToLive { get; set; }

        [FieldOrder(4)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore]
        public ushort Length { get; set; }

        [FieldOrder(5)]
        [FieldLength(nameof(Length))]
        [SubtypeFactory(nameof(Type), typeof(RecordFactory))]
        [JsonIgnore]
        public IRecord Record { get; set; }

        [Ignore]
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }

}

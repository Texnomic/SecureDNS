using BinarySerialization;
using System.Text.Json.Serialization;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Models
{
    public class Question
    {
        [FieldOrder(0)]
        //[ItemSubtypeFactory(nameof(Domain), typeof(DomainFactory))]
        [JsonIgnore]
        public Domain Domain { get; set; }

        [Ignore]
        [JsonPropertyName("name")]
        public string Name
        {
            get => Domain.Name;
            set => Domain = new Domain(value);
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
    }
}

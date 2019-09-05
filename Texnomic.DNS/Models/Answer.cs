using BinarySerialization;
using System;
using System.Text.Json.Serialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Records;

namespace Texnomic.DNS.Models
{
    public class Answer : Question, IAnswer
    {
        [FieldOrder(3)]
        [FieldBitLength(32)]
        [FieldEndianness(Endianness.Big)]
        [JsonPropertyName("TTL")]
        public uint TTL { get; set; }

        [Ignore]
        [JsonIgnore]
        public TimeSpan TimeToLive
        {
            get => new TimeSpan(0, 0, (int) TTL);
            set => throw new NotImplementedException();
        }

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

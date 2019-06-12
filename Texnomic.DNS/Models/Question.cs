using BinarySerialization;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Question
    {
        [FieldOrder(16)]
        public Domain Domain { get; set; }

        [FieldOrder(22)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public RecordType Type { get; set; }

        [FieldOrder(23)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public RecordClass Class { get; set; }
    }
}

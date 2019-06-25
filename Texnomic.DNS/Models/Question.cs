using BinarySerialization;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Question
    {
        [FieldOrder(0)]
        public Domain Domain { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public RecordType Type { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public RecordClass Class { get; set; }
    }
}

using BinarySerialization;
using System.Text;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Label
    {
        [FieldOrder(18)]
        [FieldBitLength(2)]
        public LabelType LabelType { get; set; }

        [FieldOrder(19)]
        [FieldBitLength(6)]
        [FieldEndianness(Endianness.Little)]
        [SerializeWhen(nameof(LabelType), LabelType.Normal)]
        public ushort Count { get; set; }

        [FieldOrder(20)]
        [FieldBitLength(6)]
        [FieldEndianness(Endianness.Little)]
        [SerializeWhen(nameof(LabelType), LabelType.Compressed)]
        public ushort Pointer { get; set; }

        [FieldOrder(21)]
        [FieldCount(nameof(Count))]
        [FieldEncoding(nameof(Encoding.ASCII))]
        public string Text { get; set; }
    }
}

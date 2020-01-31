using BinarySerialization;

namespace Texnomic.DNS.Models
{
    public class BitMap
    {
        [FieldOrder(0), FieldBitLength(8)]
        public byte WindowBlock { get; set; }

        [FieldOrder(1), FieldBitLength(8)]
        public byte Length { get; set; }

        [FieldOrder(2), FieldLength(nameof(Length))]
        public Map Map { get; set; }
    }
}
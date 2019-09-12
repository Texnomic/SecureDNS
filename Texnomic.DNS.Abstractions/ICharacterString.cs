using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface ICharacterString
    {
        [FieldOrder(0)]
        [FieldBitLength(8)]
        [FieldEndianness(Endianness.Big)]
        byte Length { get; set; }

        [FieldOrder(2)]
        [FieldLength(nameof(Length))]
        string Text { get; set; }
    }
}

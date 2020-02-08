using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// Character-String is a single length octet followed by that number of characters.
    /// <see cref="https://tools.ietf.org/html/rfc1035#section-3.3"/>
    /// </summary>
    public class CharacterString : ICharacterString
    {
        [FieldOrder(0), FieldBitLength(8), FieldEndianness(Endianness.Big)]
        public byte Length { get; set; }

        [FieldOrder(2), FieldLength(nameof(Length))]
        public string Value { get; set; }

        public static implicit operator string(CharacterString CharacterString)
        {
            return CharacterString.Value;
        }

        public override string ToString() => Value;
    }
}

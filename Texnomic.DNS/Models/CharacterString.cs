using BinarySerialization;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// Character-String is a single length octet followed by that number of characters.
    /// <see cref="https://tools.ietf.org/html/rfc1035#section-3.3"/>
    /// </summary>
    [LogAsScalar(true)]
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

        public static implicit operator CharacterString(string String)
        {
            return new CharacterString()
            {
                Length = (byte)String.Length,
                Value = String,
            };
        }

        public override string ToString() => Value;
    }
}

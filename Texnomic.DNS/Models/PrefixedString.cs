using BinarySerialization;
using Destructurama.Attributed;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#common-definitions"/>
    /// </summary>
    [LogAsScalar(true)]
    public class PrefixedString
    {
        [FieldOrder(0), FieldBitLength(8), FieldEndianness(Endianness.Little)]
        public byte Length { get; set; }

        [FieldOrder(2), FieldLength(nameof(Length))]
        public string Value { get; set; }

        public static implicit operator string(PrefixedString PrefixedString)
        {
            return PrefixedString?.Value;
        }

        public override string ToString() => Value;
    }
}

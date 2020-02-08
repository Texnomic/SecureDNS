using BinarySerialization;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#common-definitions"/>
    /// </summary>
    public class PrefixedArray<T>
    {
        [FieldOrder(0), FieldBitLength(8), FieldEndianness(Endianness.Little)]
        public byte Length { get; set; }

        [FieldOrder(2), FieldCount(nameof(Length))]
        public T[] Value { get; set; }

        public T this[int Index] => Value[Index];
    }
}

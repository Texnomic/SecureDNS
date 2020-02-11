using BinarySerialization;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#common-definitions"/>
    /// </summary>
    public class PrefixedArray<T>
    {
        [FieldOrder(0), FieldBitLength(8), FieldEndianness(Endianness.Little)]
        public byte Count { get; set; }

        [FieldOrder(2), FieldCount(nameof(Count))]
        public T[] Value { get; set; }

        [Ignore]
        public T this[int Index]
        {
            get => Value[Index];
            set => Value[Index] = value;
        }
    }
}

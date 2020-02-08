using System;
using BinarySerialization;
using Destructurama.Attributed;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#common-definitions"/>
    /// </summary>
    [LogAsScalar(true)]
    public class PrefixedByteArray
    {
        [FieldOrder(0), FieldBitLength(8), FieldEndianness(Endianness.Little)]
        public byte Length { get; set; }

        [FieldOrder(2), FieldLength(nameof(Length))]
        public byte[] Value { get; set; }

        public static implicit operator byte[](PrefixedByteArray PrefixedByteArray)
        {
            return PrefixedByteArray.Value;
        }

        public static implicit operator ReadOnlySpan<byte>(PrefixedByteArray PrefixedByteArray)
        {
            return PrefixedByteArray.Value;
        }

        public override string ToString() => Convert.ToBase64String(Value);
    }
}

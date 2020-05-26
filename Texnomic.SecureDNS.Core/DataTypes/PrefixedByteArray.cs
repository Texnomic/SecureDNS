using System;
using Destructurama.Attributed;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#common-definitions"/>
    /// </summary>
    [LogAsScalar(true)]
    public class PrefixedByteArray
    {
        public byte Length { get; set; }

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

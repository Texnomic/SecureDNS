using BinarySerialization;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dns-over-tls-stamps"/>
    /// </summary>
    public class DoTStamp: PlainStamp
    {
        [FieldOrder(0)]
        public PrefixedArray<PrefixedByteArray> Hash { get; set; }

        [FieldOrder(1)]
        public PrefixedString Hostname { get; set; }
    }
}

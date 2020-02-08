using BinarySerialization;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dns-over-https-stamps"/>
    /// </summary>
    public class DoHStamp: DoTStamp
    {
        [FieldOrder(0)]
        public PrefixedString Path { get; set; }
    }
}

using BinarySerialization;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dnscrypt-stamps"/>
    /// </summary>
    public class DNSCryptStamp : DoUStamp
    {
        [FieldOrder(0)]
        public PrefixedByteArray PublicKey { get; set; }

        [FieldOrder(1)]
        public PrefixedString ProviderName { get; set; }
    }
}

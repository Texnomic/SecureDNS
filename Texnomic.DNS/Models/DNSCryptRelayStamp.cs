using BinarySerialization;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#anonymized-dnscrypt-relay-stamps"/>
    /// </summary>
    public class DNSCryptRelayStamp
    {
        [FieldOrder(0)] 
        public PrefixedString Address { get; set; }
    }
}

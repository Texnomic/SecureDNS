using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#anonymized-dnscrypt-relay-stamps"/>
    /// </summary>
    public class DNSCryptRelayStamp : IStamp
    {
        public PrefixedString Address { get; set; }
    }
}

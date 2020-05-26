namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dnscrypt-stamps"/>
    /// </summary>
    public class DNSCryptStamp : DoUStamp
    {
        public PrefixedByteArray PublicKey { get; set; }

        public PrefixedString ProviderName { get; set; }
    }
}

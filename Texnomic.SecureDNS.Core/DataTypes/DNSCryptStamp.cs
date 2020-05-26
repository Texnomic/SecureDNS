namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dnscrypt-stamps"/>
    /// </summary>
    public class DNSCryptStamp : DoUStamp
    {
        public byte[] PublicKey { get; set; }

        public string ProviderName { get; set; }
    }
}

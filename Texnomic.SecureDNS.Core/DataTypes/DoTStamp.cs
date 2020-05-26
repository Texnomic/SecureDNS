namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dns-over-tls-stamps"/>
    /// </summary>
    public class DoTStamp: DoUStamp
    {
        public PrefixedArray<PrefixedByteArray> Hash { get; set; }

        public PrefixedString Hostname { get; set; }
    }
}

namespace Texnomic.SecureDNS.Core.DataTypes;

/// <summary>
/// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dns-over-tls-stamps"/>
/// </summary>
public class DoTStamp: DoUStamp
{
    public byte[] Hash { get; set; }

    public string Hostname { get; set; }
}
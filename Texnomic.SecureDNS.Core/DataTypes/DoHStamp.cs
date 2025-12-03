namespace Texnomic.SecureDNS.Core.DataTypes;

/// <summary>
/// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#dns-over-https-stamps"/>
/// </summary>
public class DoHStamp: DoTStamp
{
    public string Path { get; set; }
}
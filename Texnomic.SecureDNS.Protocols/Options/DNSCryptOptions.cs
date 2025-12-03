namespace Texnomic.SecureDNS.Protocols.Options;

public class DNSCryptOptions : IOptions
{
    public string Stamp { get; set; }

    public TimeSpan Timeout { get; set; }

    public DNSCryptStamp DNSCryptStamp => DnSerializer.Deserialize(Stamp).Value as DNSCryptStamp;
}
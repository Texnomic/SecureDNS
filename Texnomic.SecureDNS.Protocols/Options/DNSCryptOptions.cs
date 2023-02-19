using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Core.DataTypes;
using Texnomic.SecureDNS.Serialization;

namespace Texnomic.SecureDNS.Protocols.Options;

public class DNSCryptOptions : IOptions
{
    public string Stamp { get; set; }

    public TimeSpan Timeout { get; set; }

    public DNSCryptStamp DNSCryptStamp => DnSerializer.Deserialize(Stamp).Value as DNSCryptStamp;
}
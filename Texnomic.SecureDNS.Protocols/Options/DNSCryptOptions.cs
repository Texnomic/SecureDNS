using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Protocols.Options
{
    public class DNSCryptOptions : IOptions
    {
        public string Stamp { get; set; }

        public int Timeout { get; set; } = 2000;
    }
}

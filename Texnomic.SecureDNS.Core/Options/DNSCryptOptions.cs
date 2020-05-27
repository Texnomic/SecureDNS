using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Options
{
    public class DNSCryptOptions : IOptions
    {
        public string Stamp { get; set; }

        public int Timeout { get; set; } = 2000;
    }
}

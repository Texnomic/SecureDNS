using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Options
{
    public class TCPOptions : IOptions
    {
        public int Timeout { get; set; } = 2000;

        public string Host { get; set; } = "dns.google";

        public int Port { get; set; } = 853;
    }
}

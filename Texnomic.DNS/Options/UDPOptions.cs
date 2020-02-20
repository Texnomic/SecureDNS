using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Options
{
    public class UDPOptions : IOptions
    {
        public int Timeout { get; set; } = 2000;

        public string Host { get; set; } = "8.8.8.8";

        public int Port { get; set; } = 53;
    }
}

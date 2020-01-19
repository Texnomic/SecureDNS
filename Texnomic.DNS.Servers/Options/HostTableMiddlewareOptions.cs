using System.Collections.Generic;

namespace Texnomic.DNS.Servers.Options
{
    public class HostTableMiddlewareOptions
    {
        public Dictionary<string, string> HostTable { get; set; } = new Dictionary<string, string>()
        {
            { "dns.google", "8.8.8.8" }
        };

        public int TimeToLive { get; set; } = 30;
    }
}

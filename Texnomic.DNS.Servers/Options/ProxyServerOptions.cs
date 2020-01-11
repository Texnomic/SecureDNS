using System;
using System.Net;

namespace Texnomic.DNS.Servers.Options
{
    public class ProxyServerOptions
    {
        public IPEndPoint IPEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 53);

        public int Threads { get; set; } = Environment.ProcessorCount;
    }
}

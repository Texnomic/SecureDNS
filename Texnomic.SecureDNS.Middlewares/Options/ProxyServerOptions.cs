using System;
using System.Net;
using System.Text.Json.Serialization;

namespace Texnomic.SecureDNS.Middlewares.Options
{
    public class ProxyServerOptions
    {
        public string Address { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 53;

        [JsonIgnore]
        public IPEndPoint IPEndPoint => new IPEndPoint(IPAddress.Parse(Address), Port);

        public int Threads { get; set; } = Environment.ProcessorCount;
    }
}

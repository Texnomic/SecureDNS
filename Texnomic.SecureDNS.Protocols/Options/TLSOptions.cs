using System;
using System.Net;
using System.Text.Json.Serialization;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Protocols.Options
{
    public class TLSOptions : IOptions
    {
        [JsonConverter(typeof(JsonIPEndPointConverter))]
        public IPEndPoint IPv4EndPoint { get; set; } = new IPEndPoint(IPAddress.Parse("8.8.8.8"), 853);

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);

        public string CommonName { get; set; } = "dns.google";

        public string Thumbprint { get; set; }
    }
}

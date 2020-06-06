using System;
using System.Net;
using System.Text.Json.Serialization;

using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Protocols.Options
{
    public class TCPOptions : IOptions
    {
        [JsonConverter(typeof(JsonIPEndPointConverter))]
        public IPEndPoint IPv4EndPoint { get; set; } = new IPEndPoint(IPAddress.Parse("1.1.1.1"), 53);

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);

    }
}

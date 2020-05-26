using System.Text.Json.Serialization;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Core.DataTypes;

namespace Texnomic.SecureDNS.Core.Options
{
    public class DNSCryptOptions : IOptions
    {
        [JsonConverter(typeof(JsonConverter<DnStamp>))]
        public DnStamp Stamp { get; set; } = DnStamp.FromString("sdns://AQYAAAAAAAAADTkuOS45LjEwOjg0NDMgZ8hHuMh1jNEgJFVDvnVnRt803x2EwAuMRwNo34Idhj4ZMi5kbnNjcnlwdC1jZXJ0LnF1YWQ5Lm5ldA");

        public int Timeout { get; set; } = 2000;
    }
}

using System.Text.Json.Serialization;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Options
{
    public class DNSCryptOptions
    {
        [JsonConverter(typeof(JsonConverter<Stamp>))]
        public Stamp Stamp { get; set; } = new Stamp("sdns://AQYAAAAAAAAADTkuOS45LjEwOjg0NDMgZ8hHuMh1jNEgJFVDvnVnRt803x2EwAuMRwNo34Idhj4ZMi5kbnNjcnlwdC1jZXJ0LnF1YWQ5Lm5ldA");

        public int Timeout { get; set; } = 2000;
    }
}

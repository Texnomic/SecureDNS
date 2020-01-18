using System;
using System.Net;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Texnomic.DNS.Servers.JsonConverters
{
    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        private readonly Type[] Types = new Type[] { typeof(IPAddress) };

        public override bool CanConvert(Type TargetType) => Types.Any(Type => Type == TargetType);

        public override IPAddress Read(ref Utf8JsonReader Reader, Type typeToConvert, JsonSerializerOptions Options)
        {
            return IPAddress.Parse(Reader.GetString());
        }

        public override void Write(Utf8JsonWriter Writer, IPAddress Value, JsonSerializerOptions Options)
        {
            Writer.WriteStringValue(Value.ToString());
        }
    }
}

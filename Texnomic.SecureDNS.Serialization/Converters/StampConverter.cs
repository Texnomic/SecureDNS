using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Texnomic.SecureDNS.Core.DataTypes;

namespace Texnomic.SecureDNS.Serialization.Converters
{
    public class StampConverter : JsonConverter<DnStamp>
    {
        public override DnStamp Read(ref Utf8JsonReader Reader, Type TypeToConvert, JsonSerializerOptions Options)
        {
            var Stamp = Reader.GetString();

            if (!Stamp.StartsWith("sdns://")) throw new ArgumentException("Stamp Uri Must Start With SDNS://");

            var Bytes = Decode(Stamp[7..]);

            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter Writer, DnStamp Value, JsonSerializerOptions Options)
        {
            throw new NotImplementedException();
        }

        private static string Encode(byte[] Stamp)
        {
            return Convert.ToBase64String(Stamp)
                .Replace("=", "")
                .Replace("/", "_")
                .Replace("+", "-");
        }

        private static string ToBase64(string Stamp)
        {
            return Stamp
                .PadRight(Stamp.Length + (4 - Stamp.Length % 4) % 4, '=')
                .Replace("_", "/")
                .Replace("-", "+");
        }

        private static byte[] Decode(string Stamp)
        {
            return Convert.FromBase64String(ToBase64(Stamp));
        }
    }
}

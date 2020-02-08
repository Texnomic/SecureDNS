using BinarySerialization;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Converters
{
    public class StampConverter : JsonConverter<Stamp>
    {
        private readonly BinarySerializer BinarySerializer;

        public StampConverter()
        {
            BinarySerializer = new BinarySerializer();
        }

        public override Stamp Read(ref Utf8JsonReader Reader, Type TypeToConvert, JsonSerializerOptions Options)
        {
            var Stamp = Reader.GetString();

            if (!Stamp.StartsWith("sdns://")) throw new ArgumentException("Stamp Uri Must Start With SDNS://");

            var Bytes = Decode(Stamp[7..]);

            return BinarySerializer.Deserialize<Stamp>(Bytes);
        }

        public override void Write(Utf8JsonWriter Writer, Stamp Value, JsonSerializerOptions Options)
        {
            var Bytes = BinarySerializer.Serialize(Value);

            var Stamp = Encode(Bytes);

            Writer.WriteStringValue($"sdns://{Stamp}");
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

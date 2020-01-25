using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Converters
{
    public class StampConveter : JsonConverter<Stamp>
    {
        private readonly BinarySerializer BinarySerializer;

        public StampConveter()
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


        /// <summary>
        /// source: https://gist.github.com/igorushko/cccef0561aea7e46ae52bc62270b2b61
        /// </summary>
        private static string Encode(byte[] arg)
        {
            if (arg == null) throw new ArgumentNullException("arg");

            var s = Convert.ToBase64String(arg);
            return s
                .Replace("=", "")
                .Replace("/", "_")
                .Replace("+", "-");
        }

        /// <summary>
        /// source: https://gist.github.com/igorushko/cccef0561aea7e46ae52bc62270b2b61
        /// </summary>
        private static string ToBase64(string arg)
        {
            if (arg == null) throw new ArgumentNullException("arg");

            var s = arg
                .PadRight(arg.Length + (4 - arg.Length % 4) % 4, '=')
                .Replace("_", "/")
                .Replace("-", "+");

            return s;
        }

        /// <summary>
        /// source: https://gist.github.com/igorushko/cccef0561aea7e46ae52bc62270b2b61
        /// </summary>
        private static byte[] Decode(string arg)
        {
            var decrypted = ToBase64(arg);

            return Convert.FromBase64String(decrypted);
        }
    }
}

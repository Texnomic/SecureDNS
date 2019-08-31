using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Converters
{
    public class RecursionDesiredConverter : JsonConverter<RecursionDesired>
    {
        public override RecursionDesired Read(ref Utf8JsonReader Reader, Type TypeToConvert, JsonSerializerOptions Options)
        {
            return Reader.TokenType == JsonTokenType.True ? RecursionDesired.Recursive : RecursionDesired.Iterative;
        }

        public override void Write(Utf8JsonWriter Writer, RecursionDesired Value, JsonSerializerOptions Options)
        {
            Writer.WriteStringValue(Value.ToString());
        }
    }
}

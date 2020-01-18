using Newtonsoft.Json;
using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace Texnomic.DNS.Servers.JsonConverters
{
    public class DictionaryStringIPAddressConverter : JsonConverter
    {
        private readonly Type[] Types = new Type[] { typeof(Dictionary<string, IPAddress>) };

        public override bool CanConvert(Type TargetType) => Types.Any(Type => Type == TargetType);

        public override bool CanRead => true;

        public override object ReadJson(JsonReader Reader, Type ObjectType, object ExistingValue, JsonSerializer Serializer)
        {
            if (Reader.TokenType == JsonToken.StartArray)
            {
                Reader.Read();

                if (Reader.TokenType == JsonToken.EndArray || Reader.TokenType == JsonToken.Null)
                {
                    return new Dictionary<string, IPAddress>();
                }

                if (Reader.TokenType == JsonToken.StartObject)
                {
                    var Dictionary = new Dictionary<string, IPAddress>();

                    Reader.Read();

                    while (Reader.TokenType != JsonToken.EndObject)
                    {
                        if (Reader.TokenType != JsonToken.PropertyName) throw new JsonSerializationException("Unexpected JSON Token.");

                        var Domain = (string)Reader.Value;

                        Reader.Read();

                        if (Reader.TokenType != JsonToken.String) throw new JsonSerializationException("Unexpected JSON Token.");

                        var IP = IPAddress.Parse((string)Reader.Value);

                        Dictionary.Add(Domain, IP);

                        Reader.Read();
                    }

                    return Dictionary;
                }
                else
                {
                    throw new JsonSerializationException("Unexpected JSON Token.");
                }
            }
            else
            {
                throw new JsonSerializationException("Unexpected JSON Token.");
            }
        }

        public override void WriteJson(JsonWriter Writer, object Value, JsonSerializer Serializer)
        {
            var Dictionary = (Dictionary<string, IPAddress>)Value;

            Writer.WriteStartArray();

            foreach (var KeyPair in Dictionary)
            {
                Writer.WriteStartObject();

                Writer.WriteValue(KeyPair.Key);
                Writer.WriteValue(KeyPair.Value.ToString());

                Writer.WriteEndObject();
            }

            Writer.WriteEndArray();
        }
    }
}

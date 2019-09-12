using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace Texnomic.FilterLists
{
    public class NewtonsoftJsonSerializer : IRestSerializer
    {
        public string Serialize(object Obj) => JsonConvert.SerializeObject(Obj);

        public string Serialize(Parameter Parameter) => JsonConvert.SerializeObject(Parameter.Value);

        public T Deserialize<T>(IRestResponse Response) => JsonConvert.DeserializeObject<T>(Response.Content);

        public string[] SupportedContentTypes { get; } =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}

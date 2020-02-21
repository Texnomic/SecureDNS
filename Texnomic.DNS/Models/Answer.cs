using BinarySerialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Factories;

namespace Texnomic.DNS.Models
{
    public class Answer : Question, IAnswer
    {
        [FieldOrder(3)]
        [SubtypeFactory(nameof(TimeToLive), typeof(TimeToLiveFactory), BindingMode = BindingMode.OneWayToSource)]
        [LogAsScalar(true)]
        [JsonPropertyName("TTL")]
        public ITimeToLive TimeToLive { get; set; }

        [FieldOrder(4)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        [JsonIgnore, NotLogged]
        public ushort Length { get; set; }

        [FieldOrder(5)]
        [FieldLength(nameof(Length))]
        [SubtypeFactory(nameof(Type), typeof(RecordFactory))]
        [JsonIgnore]
        public IRecord Record { get; set; }

        [Ignore]
        [NotLogged]
        [JsonPropertyName("data")]
        public string Data { get; set; }

        public new static Answer FromArray(byte[] Bytes)
        {
            var Serializer = new BinarySerializer();
            return Serializer.Deserialize<Answer>(Bytes);
        }

        public new static async Task<Answer> FromArrayAsync(byte[] Bytes)
        {
            var Serializer = new BinarySerializer();
            return await Serializer.DeserializeAsync<Answer>(Bytes);
        }
    }

}

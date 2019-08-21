using BinarySerialization;
using Nerdbank.Streams;
using System.Buffers;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Question
    {
        [Ignore]
        protected static readonly BinarySerializer Serializer = new BinarySerializer();

        [FieldOrder(0)]
        public Domain Domain { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public RecordType Type { get; set; }

        [FieldOrder(2)]
        [FieldBitLength(16)]
        [FieldEndianness(Endianness.Big)]
        public RecordClass Class { get; set; }

        public byte[] ToArray()
        {
            using var Stream = new MemoryStream();
            Serializer.Serialize(Stream, this);
            return Stream.ToArray();
        }

        public static Question FromArray(byte[] Data)
        {
            return Serializer.Deserialize<Question>(Data);
        }

        public static Question FromArray(ReadOnlySequence<byte> Data)
        {
            return Serializer.Deserialize<Question>(Data.AsStream());
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message FromJson(string Json)
        {
            return JsonSerializer.Deserialize<Message>(Json);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}

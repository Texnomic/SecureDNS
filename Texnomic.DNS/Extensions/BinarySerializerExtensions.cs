using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using BinarySerialization;
using Nerdbank.Streams;

namespace Texnomic.DNS.Extensions
{
    public static class BinarySerializerExtensions
    {
        public static async ValueTask<byte[]> SerializeAsync(this BinarySerializer Serializer, object Object)
        {
            await using var Stream = new MemoryStream();
            await Serializer.SerializeAsync(Stream, Object);
            return Stream.ToArray();
        }

        public static byte[] Serialize(this BinarySerializer Serializer, object Object)
        {
            using var Stream = new MemoryStream();
            Serializer.Serialize(Stream, Object);
            return Stream.ToArray();
        }

        public static async ValueTask<T> DeserializeAsync<T>(this BinarySerializer Serializer, ReadOnlySequence<byte> ReadOnlySequence)
        {
            return await Serializer.DeserializeAsync<T>(ReadOnlySequence.AsStream());
        }
    }
}

using System.IO;
using System.Threading.Tasks;
using BinarySerialization;

namespace Texnomic.DNS.Extensions
{
    public static class BinarySerializerExtensions
    {
        public static async Task<byte[]> SerializeAsync(this BinarySerializer Serializer, object Object)
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
    }
}

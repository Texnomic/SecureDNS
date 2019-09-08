using System.IO;
using System.Net;
using BinarySerialization;

namespace Texnomic.DNS.Models
{
    public class IPv6Address : IBinarySerializable
    {
        [Ignore]
        public IPAddress Value { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Buffer = new byte[16];
            Stream.Read(Buffer);
            Value = new IPAddress(Buffer);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            Stream.Write(Value.GetAddressBytes());
        }
    }
}

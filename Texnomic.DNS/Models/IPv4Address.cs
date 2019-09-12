using System.IO;
using System.Net;
using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Models
{
    public class IPv4Address : IBinarySerializable, IIPv4Address
    {
        [Ignore]
        public IPAddress Value { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Buffer = new byte[4];
            Stream.Read(Buffer);
            Value = new IPAddress(Buffer);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            Stream.Write(Value.GetAddressBytes());
        }
    }
}

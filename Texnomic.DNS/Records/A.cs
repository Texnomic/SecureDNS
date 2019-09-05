using System.IO;
using System.Net;
using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Records
{
    public class A : IRecord, IBinarySerializable
    {
        [Ignore]
        public IPAddress IPAddress { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Buffer = new byte[4];
            Stream.Read(Buffer);
            IPAddress = new IPAddress(Buffer);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            Stream.Write(IPAddress.GetAddressBytes());
        }
    }
}

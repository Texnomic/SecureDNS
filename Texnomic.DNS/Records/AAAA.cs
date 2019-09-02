using System;
using System.IO;
using System.Net;
using BinarySerialization;

namespace Texnomic.DNS.Records
{
    public class AAAA : IRecord, IBinarySerializable
    {
        [Ignore]
        public IPAddress IPAddress { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Buffer = new byte[16];
            Stream.Read(Buffer);
            IPAddress = new IPAddress(Buffer);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            Stream.Write(IPAddress.GetAddressBytes());
        }
    }
}

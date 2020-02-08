using System.IO;
using System.Net;
using BinarySerialization;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Models
{
    [LogAsScalar(true)]
    public class IPv6Address : IBinarySerializable, IIPv6Address
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

        public static implicit operator string(IPv6Address IPv6Address)
        {
            return IPv6Address.Value.ToString();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

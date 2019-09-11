using System;
using System.IO;
using BinarySerialization;

namespace Texnomic.DNS.Models
{
    public class Epoch : IBinarySerializable
    {
        [Ignore]
        public DateTime Value { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Buffer = new byte[4];
            Stream.Read(Buffer);
            Array.Reverse(Buffer);
            var Seconds = BitConverter.ToUInt32(Buffer);
            Value = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Seconds);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Seconds = Value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var Bytes = BitConverter.GetBytes((uint)Seconds);
            Array.Reverse(Bytes);
            Stream.Write(Bytes);
        }
    }
}

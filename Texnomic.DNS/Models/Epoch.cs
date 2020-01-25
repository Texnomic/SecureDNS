using System;
using System.IO;
using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Models
{
    public class Epoch : IEpoch, IBinarySerializable
    {
        [Ignore]
        public DateTime Value { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Buffer = new byte[4];
            Stream.Read(Buffer);
            if (Endianness == Endianness.Big) Array.Reverse(Buffer);
            var Seconds = BitConverter.ToUInt32(Buffer);
            Value = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Seconds);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Seconds = Value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var Bytes = BitConverter.GetBytes((uint)Seconds);
            if (Endianness == Endianness.Big) Array.Reverse(Bytes);
            Stream.Write(Bytes);
        }

        public override string ToString()
        {
            return Value.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}

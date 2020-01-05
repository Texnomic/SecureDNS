using System;
using System.IO;
using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Models
{
    public class TimeToLive : IBinarySerializable, ITimeToLive
    {
        [Ignore]
        public TimeSpan Value { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Buffer = new byte[4];
            Stream.Read(Buffer);
            Array.Reverse(Buffer);
            var TTL = BitConverter.ToUInt32(Buffer);
            Value = new TimeSpan(0, 0, (int)TTL);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Bytes = BitConverter.GetBytes((uint)Value.TotalSeconds);
            Array.Reverse(Bytes);
            Stream.Write(Bytes);
        }

        public override string ToString()
        {
            return Value.ToString(@"dd\.hh\:mm\:ss");
        }
    }
}

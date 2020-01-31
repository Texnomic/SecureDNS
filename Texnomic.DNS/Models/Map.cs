using System.Collections.Generic;
using System.IO;
using BinarySerialization;
using BitStreams;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Models
{
    public class Map : IBinarySerializable
    {
        [Ignore] 
        public List<RecordType> Value { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            var BitMap = (BitMap)Context.ParentContext.Value;

            var BitStream = new BitStream(Stream, true);

            Value = new List<RecordType>();

            for (var I = 0; I < BitMap.Length * 8; I++)
            {
                if (BitStream.ReadBit())
                {
                    Value.Add((RecordType) (I + BitMap.WindowBlock * 256));
                }
            }
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            var BitMap = (BitMap) Context.ParentContext.Value;

            var Buffer = new byte[BitMap.Length * 8 + 2];

            var BitStream = new BitStream(Buffer, true);

            for (var I = 0; I < BitMap.Length * 8; I++)
            {
                BitStream.WriteBit(Value.Contains((RecordType) (I + BitMap.WindowBlock * 256)));
            }

            Stream.Write(BitStream.GetStreamData());
        }
    }
}

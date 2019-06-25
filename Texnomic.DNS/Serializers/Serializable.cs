using BinarySerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Texnomic.DNS.Serializers
{
    public class Serializable<T> : IBinarySerializable
    {
        [Ignore]
        public T Value { get; set; }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
       
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
           
        }
    }
}

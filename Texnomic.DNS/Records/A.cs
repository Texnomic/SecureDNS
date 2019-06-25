using BinarySerialization;
using System.Net;

namespace Texnomic.DNS.Records
{
    public class A : IRecord
    {
        [FieldOrder(0)]
        [FieldBitLength(32)]
        [FieldEndianness(Endianness.Little)]
        public uint Data { get; set; }

        [Ignore]
        public IPAddress IPAddress => new IPAddress(Data);
    }
}

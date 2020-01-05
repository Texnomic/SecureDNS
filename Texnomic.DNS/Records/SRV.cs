using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5


    /// <summary>
    /// Service Resource Record <see href="https://tools.ietf.org/html/rfc2052">(SRV)</see>
    /// </summary>
    public class SRV : IRecord
    {
        [FieldOrder(0), FieldEndianness(Endianness.Big)] 
        public ushort Priority { get; set; }

        [FieldOrder(1), FieldEndianness(Endianness.Big)] 
        public ushort Weight { get; set; }

        [FieldOrder(2), FieldEndianness(Endianness.Big)] 
        public ushort Port { get; set; }

        [FieldOrder(3)] 
        public Domain Target { get; set; }
    }
}

using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                  PREFERENCE                   |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                   EXCHANGE                    /
    // /                                               /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Mail Exchange Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.9">(MX)</see>
    /// </summary>
    public class MX : IRecord
    {
        [FieldOrder(0), FieldCount(16), FieldEndianness(Endianness.Big)]
        public short Preference { get; set; }

        [FieldOrder(1)] 
        public Domain Domain { get; set; }
    }
}

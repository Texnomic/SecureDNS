using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    ADDRESS                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |       PROTOCOL        |                       |
    // +--+--+--+--+--+--+--+--+                       |
    // |                                               |
    // /                   <BIT MAP>                   /
    // /                                               /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Well Known Services Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.4.2">(WKS)</see>
    /// </summary>
    public class WKS : IRecord
    {
        [FieldOrder(0), FieldLength(32)] 
        public IPv4Address Address { get; set; }

        [FieldOrder(1), FieldLength(8)]
        public Protocol Protocol { get; set; }

        [FieldOrder(2)] 
        public BitMap BitMap { get; set; }
    }
}

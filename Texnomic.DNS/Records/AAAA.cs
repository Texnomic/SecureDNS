using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    ADDRESS                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// 128-bit Internet Address Resource Record <see href="https://tools.ietf.org/html/rfc3596">(AAAA)</see>
    /// </summary>
    public class AAAA : IRecord
    {
        [FieldOrder(0), FieldLength(128)]
        public IPv6Address Address { get; set; }
    }
}

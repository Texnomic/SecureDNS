using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                 <anything>                    /
    // /                                               /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Anything Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.10">(NULL)</see>
    /// </summary>
    public class NULL : IRecord
    {
        [FieldOrder(0)]
        public byte[] Anything { get; set; }
    }
}

using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                   TXT-DATA                    /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Text Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.14">(TXT)</see>
    /// </summary>
    public class TXT : IRecord
    {
        [FieldOrder(0)]
        public string Text { get; set; }

        [FieldOrder(1)]
        [SerializeWhen(nameof(Text), "|DNSC")]
        public Certificate Certificate { get; set; }
    }
}

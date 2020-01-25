using BinarySerialization;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //   +0-1-2-3-4-5-6-7-|0-1-2-3-4-5-6-7-|
    //   | Flags          | Tag Length = n |
    //   +----------------+----------------+...+---------------+
    //   | Tag char 0     | Tag char 1     |...| Tag char n-1  |
    //   +----------------+----------------+...+---------------+
    //   +----------------+----------------+.....+----------------+
    //   | Value byte 0   | Value byte 1   |.....| Value byte m-1 |
    //   +----------------+----------------+.....+----------------+

    /// <summary>
    /// Certification Authority Authorization Record <see href="https://tools.ietf.org/html/rfc6844#section-5.1">(CAA)</see>
    /// </summary>
    public class CAA : IRecord
    {
        [FieldOrder(0), FieldBitLength(8)] 
        public ushort Flags { get; set; }

        [FieldOrder(1)]
        [LogAsScalar(true)]
        public CharacterString Tags { get; set; }

        [FieldOrder(2)]
        public string Value { get; set; }
    }
}

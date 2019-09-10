using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // MSB                                           LSB
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    //  0|        VERSION        |         SIZE          |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    //  2|       HORIZ PRE       |       VERT PRE        |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    //  4|                   LATITUDE                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    //  6|                   LATITUDE                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    //  8|                   LONGITUDE                   |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // 10|                   LONGITUDE                   |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // 12|                   ALTITUDE                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // 14|                   ALTITUDE                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Location Resource Record <see href="https://tools.ietf.org/html/rfc1876#section-2">(LOC)</see>
    /// </summary>
    public class LOC: IRecord
    {
        [FieldOrder(0), FieldEndianness(Endianness.Big)]
        public byte Version { get; set; }

        [FieldOrder(0), FieldEndianness(Endianness.Big)]
        public byte Size { get; set; }

        [FieldOrder(0), FieldEndianness(Endianness.Big)]
        public byte HorizontalPrecision { get; set; }

        [FieldOrder(0), FieldEndianness(Endianness.Big)]
        public byte VerticalPrecision { get; set; }

        [FieldOrder(0), FieldEndianness(Endianness.Big)]
        public int Latitude { get; set; }

        [FieldOrder(0), FieldEndianness(Endianness.Big)]
        public int Longitude { get; set; }

        [FieldOrder(0), FieldEndianness(Endianness.Big)]
        public int Altitude { get; set; }
    }
}

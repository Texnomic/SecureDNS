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
        [FieldOrder(0), FieldBitLength(8), FieldOffset(0), FieldEndianness(Endianness.Big)]
        public byte Length { get; set; }

        [FieldOrder(1), FieldOffset(1), FieldBitLength(32)]
        public string Magic { get; set; }

        [FieldOrder(2), FieldOffset(1), FieldLength(nameof(Length))]
        [SerializeWhenNot(nameof(Magic), "DNSC")]
        public string Text { get; set; }

        [FieldOrder(3), FieldOffset(1), FieldLength(nameof(Length))]
        [SerializeWhen(nameof(Magic), "DNSC")]
        public Certificate Certificate { get; set; }
    }
}

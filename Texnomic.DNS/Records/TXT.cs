using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Factories;

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

        //ESVersion is 2 Bytes but the first one 0x00 was taken as NULL-Terminated String in Text Field.

        [FieldOrder(1), SerializeWhen(nameof(Text), "|DNSC"), FieldBitLength(8), FieldEndianness(Endianness.Little)]
        public ushort ESVersion { get; set; }

        [FieldOrder(2), SerializeWhen(nameof(Text), "|DNSC"), FieldBitLength(16), FieldEndianness(Endianness.Little)]
        public ushort MinorVersion { get; set; }

        [FieldOrder(3), SerializeWhen(nameof(Text), "|DNSC"), FieldBitLength(512), FieldEndianness(Endianness.Little)]
        public byte[] Signature { get; set; }

        [FieldOrder(4), SerializeWhen(nameof(Text), "|DNSC"), FieldBitLength(256), FieldEndianness(Endianness.Little)]
        public byte[] PublicKey { get; set; }

        [FieldOrder(5), SerializeWhen(nameof(Text), "|DNSC"), FieldBitLength(64), FieldEndianness(Endianness.Little)]
        public byte[] ClientMagic { get; set; }

        [FieldOrder(6), SerializeWhen(nameof(Text), "|DNSC"), FieldBitLength(32), FieldEndianness(Endianness.Big)]
        public int Serial { get; set; }

        [FieldOrder(7), SerializeWhen(nameof(Text), "|DNSC"), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(StartTimeStamp), typeof(EpochFactory), BindingMode = BindingMode.OneWayToSource)]
        public IEpoch StartTimeStamp { get; set; }

        [FieldOrder(8), SerializeWhen(nameof(Text), "|DNSC"), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(EndTimeStamp), typeof(EpochFactory), BindingMode = BindingMode.OneWayToSource)]
        public IEpoch EndTimeStamp { get; set; }

        [FieldOrder(9), SerializeWhen(nameof(Text), "|DNSC"), FieldEndianness(Endianness.Little)]
        public byte[] Extensions { get; set; }
    }
}

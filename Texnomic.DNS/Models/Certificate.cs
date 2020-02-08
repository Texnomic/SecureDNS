using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Factories;

namespace Texnomic.DNS.Models
{
    public class Certificate
    {
        //ESVersion is 2 Bytes but the first one 0x00 was taken as NULL-Terminated String in Text Field.

        [FieldOrder(0), FieldBitLength(8), FieldEndianness(Endianness.Little)]
        public ushort Version { get; set; }

        [FieldOrder(1), FieldBitLength(16), FieldEndianness(Endianness.Little)]
        public ushort MinorVersion { get; set; }

        [FieldOrder(2), FieldBitLength(512), FieldEndianness(Endianness.Little)]
        public byte[] Signature { get; set; }

        [FieldOrder(3), FieldBitLength(256), FieldEndianness(Endianness.Little)]
        public byte[] PublicKey { get; set; }

        [FieldOrder(4), FieldBitLength(64), FieldEndianness(Endianness.Little)]
        public byte[] ClientMagic { get; set; }

        [FieldOrder(5), FieldBitLength(32), FieldEndianness(Endianness.Big)]
        public int Serial { get; set; }

        [FieldOrder(6), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(StartTimeStamp), typeof(EpochFactory), BindingMode = BindingMode.OneWayToSource)]
        public IEpoch StartTimeStamp { get; set; }

        [FieldOrder(7), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(EndTimeStamp), typeof(EpochFactory), BindingMode = BindingMode.OneWayToSource)]
        public IEpoch EndTimeStamp { get; set; }

        [FieldOrder(8), FieldEndianness(Endianness.Little)]
        public byte[] Extensions { get; set; }
    }
}

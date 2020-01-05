using BinarySerialization;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Factories;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                     1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
    // 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // |        Type Covered           |  Algorithm    |     Labels    |
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // |                         Original TTL                          |
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // |                      Signature Expiration                     |
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // |                      Signature Inception                      |
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // |            Key Tag            |                               /
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+         Signer's Name         /
    // /                                                               /
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // /                                                               /
    // /                            Signature                          /
    // /                                                               /
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

    /// <summary>
    /// Resource Record Signature Resource Record <see href="https://tools.ietf.org/html/rfc4034#section-3.1">(RRSIG)</see>
    /// </summary>
    public class RRSIG : IRecord
    {
        [FieldOrder(0), FieldBitLength(16), FieldEndianness(Endianness.Big)] 
        public RecordType TypeCovered { get; set; }

        [FieldOrder(1), FieldBitLength(8), FieldEndianness(Endianness.Big)]
        public Algorithm Algorithm { get; set; }

        [FieldOrder(2), FieldBitLength(8), FieldEndianness(Endianness.Big)] 
        public byte Labels { get; set; }

        [FieldOrder(3), FieldBitLength(32), FieldEndianness(Endianness.Big)] 
        public int OriginalTTL { get; set; }

        [FieldOrder(4), FieldBitLength(32), FieldEndianness(Endianness.Big)]
        [LogAsScalar(true)]
        [SubtypeFactory(nameof(SignatureExpiration), typeof(EpochFactory), BindingMode = BindingMode.OneWayToSource)]
        public IEpoch SignatureExpiration { get; set; }

        [FieldOrder(5), FieldBitLength(32), FieldEndianness(Endianness.Big)]
        [LogAsScalar(true)]
        [SubtypeFactory(nameof(SignatureInception), typeof(EpochFactory), BindingMode = BindingMode.OneWayToSource)]
        public IEpoch SignatureInception { get; set; }

        [FieldOrder(6), FieldBitLength(16), FieldEndianness(Endianness.Big)]
        public ushort KeyTag { get; set; }

        [FieldOrder(7)]
        [SubtypeFactory(nameof(SignerName), typeof(DomainFactory), BindingMode = BindingMode.OneWayToSource)]
        public IDomain SignerName { get; set; }

        [FieldOrder(8)]
        [LogAsScalar(true)]
        [SubtypeDefault(typeof(Base64String))]
        public IBase64String Signature { get; set; }
    }
}

using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.Records;
//                     1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |              flags          |S|   protocol    |   algorithm   |
// |                             |E|               |               |
// |                             |P|               |               |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |                                                               /
// /                        public key                             /
// /                                                               /
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

/// <summary>
/// Domain Name System KEY Resource Record <see href="https://tools.ietf.org/html/rfc3757#section-2">(DNSKEY)</see>
/// </summary>
public class DNSKEY : IRecord
{
    //[FieldOrder(0), FieldBitLength(1)] public bool Flag0 { get; set; }
    //[FieldOrder(1), FieldBitLength(1)] public bool Flag1 { get; set; }
    //[FieldOrder(2), FieldBitLength(1)] public bool Flag2 { get; set; }
    //[FieldOrder(3), FieldBitLength(1)] public bool Flag3 { get; set; }
    //[FieldOrder(4), FieldBitLength(1)] public bool Flag4 { get; set; }
    //[FieldOrder(5), FieldBitLength(1)] public bool Flag5 { get; set; }
    //[FieldOrder(6), FieldBitLength(1)] public bool Flag6 { get; set; }
    //[FieldOrder(7), FieldBitLength(1)] public bool Flag7 { get; set; }
    //[FieldOrder(8), FieldBitLength(1)] public bool Flag8 { get; set; }
    //[FieldOrder(9), FieldBitLength(1)] public bool Flag9 { get; set; }
    //[FieldOrder(10), FieldBitLength(1)] public bool Flag10 { get; set; }
    //[FieldOrder(11), FieldBitLength(1)] public bool Flag11 { get; set; }
    //[FieldOrder(12), FieldBitLength(1)] public bool Flag12 { get; set; }
    //[FieldOrder(13), FieldBitLength(1)] public bool Flag13 { get; set; }
    //[FieldOrder(14), FieldBitLength(1)] public bool Flag14 { get; set; }

    public ushort Flags { get; set; }

    public bool SecureEntryPoint { get; set; }

    public byte Protocol { get; set; }

    public Algorithm Algorithm { get; set; }

    public IBase64String PublicKey { get; set; }
}
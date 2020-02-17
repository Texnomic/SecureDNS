using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    /// <summary>
    /// DNSCrypt Certificate <see href="https://raw.githubusercontent.com/DNSCrypt/dnscrypt-protocol/master/DNSCRYPT-V2-PROTOCOL.txt">(TXT)</see>
    /// </summary>
    public class DNSC
    {
        [FieldOrder(0), FieldBitLength(8), FieldEndianness(Endianness.Big)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        public Certificate Certificate { get; set; }
    }
}

using BinarySerialization;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5


    /// <summary>
    /// Naming Authority Pointer Resource Record <see href="https://tools.ietf.org/html/rfc2915#section-2">(NAPTR)</see>
    /// </summary>
    public class NAPTR : IRecord
    {
        [FieldOrder(0), FieldLength(16), FieldEndianness(Endianness.Big)]
        public ushort Order { get; set; }

        [FieldOrder(1), FieldLength(16), FieldEndianness(Endianness.Big)]
        public ushort Preference { get; set; }

        [FieldOrder(2)]
        [LogAsScalar(true)]
        [SubtypeDefault(typeof(CharacterString))]
        public ICharacterString Flags { get; set; }

        [FieldOrder(3)]
        [LogAsScalar(true)]
        [SubtypeDefault(typeof(CharacterString))]
        public ICharacterString Parameters { get; set; }
    }
}

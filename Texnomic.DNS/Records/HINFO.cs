using BinarySerialization;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                      CPU                      /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                       OS                      /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Host Information Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.2">(HINFO)</see>
    /// </summary>
    public class HINFO : IRecord
    {
        [FieldOrder(0)]
        [LogAsScalar(true)]
        [SubtypeDefault(typeof(CharacterString))]
        public ICharacterString CPU { get; set; }

        [FieldOrder(1)]
        [LogAsScalar(true)]
        [SubtypeDefault(typeof(CharacterString))]
        public ICharacterString OS { get; set; }
    }
}

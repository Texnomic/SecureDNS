using System.Linq;
using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                      1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
    //  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // /                      Next Domain Name                         /
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // /                   List of Type Bit Map(s)                     /
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

    /// <summary>
    /// NextSECure Record <see href="https://tools.ietf.org/html/rfc3845#section-2.1">(NSEC)</see>
    /// </summary>
    public class NSEC : IRecord
    {
        [FieldOrder(0)] 
        public Domain NextDomain { get; set; }

        [FieldOrder(1)]
        public BitMap[] BitMaps { get; set; }

        [Ignore] 
        public RecordType[] Records => BitMaps?.SelectMany(BitMap => BitMap.Map.Value).ToArray();
    }
}

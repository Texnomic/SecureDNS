using System;
using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    public class PseudoRecord : IPseudoRecord
    {
        public IDomain Domain { get; set; }

        public RecordType Type { get; set; }

        [NotLogged] 
        public RecordClass? Class { get; set; } = null;

        [NotLogged]
        public TimeSpan? TimeToLive { get; set; } = null;

        [NotLogged] 
        public IRecord? Record { get; set; } = null;

        public ushort Size { get; set; }

        public RecordType ExtendedType { get; set; }

        public byte Version { get; set; }

        public bool DNSSEC { get; set; }

        public ushort Zero { get; set; }

        public ushort Length { get; set; }

        public byte[] Data { get; set; }

    }
}

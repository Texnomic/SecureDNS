using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Abstractions
{
    public interface IPseudoRecord : IAnswer
    {
        ushort Size { get; set; }

        RecordType ExtendedType { get; set; }

        byte Version { get; set; }

        bool DNSSEC { get; set; }

        ushort Zero { get; set; }

        byte[] Data { get; set; }
    }
}

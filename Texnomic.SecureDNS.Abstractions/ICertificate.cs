using System;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Abstractions
{
    public interface ICertificate
    {
        byte Length { get; set; }

        string Magic { get; set; }

        ESVersion Version { get; set; }

        ushort MinorVersion { get; set; }

        byte[] Signature { get; set; }

        byte[] PublicKey { get; set; }

        byte[] ClientMagic { get; set; }

        int Serial { get; set; }

        DateTime StartTimeStamp { get; set; }

        DateTime EndTimeStamp { get; set; }

        byte[] Extensions { get; set; }

    }
}

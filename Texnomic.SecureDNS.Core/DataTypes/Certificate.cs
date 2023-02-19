using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.DataTypes;

public class Certificate: ICertificate
{
    public byte Length { get; set; }

    public string Magic { get; set; }

    public ESVersion Version { get; set; }

    public ushort MinorVersion { get; set; }

    public byte[] Signature { get; set; }

    public byte[] PublicKey { get; set; }

    public byte[] ClientMagic { get; set; }

    public int Serial { get; set; }

    public DateTime StartTimeStamp { get; set; }

    public DateTime EndTimeStamp { get; set; }

    public byte[] Extensions { get; set; }
}
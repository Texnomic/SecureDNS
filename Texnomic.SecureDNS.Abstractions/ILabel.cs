using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Abstractions
{
    public interface ILabel
    {
        byte[] Bytes { get; set; }
    }
}

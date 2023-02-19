using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Abstractions;

public interface IDnStamp
{
    StampProtocol Protocol { get; set; }

    IStamp Value { get; set; }

    static IDnStamp FromString(string Stamp)
    {
        throw new NotImplementedException();
    }
}
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    public class DnStamp : IDnStamp
    {
        public StampProtocol Protocol { get; set; }

        public IStamp Value { get; set; }
    }
}

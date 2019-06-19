using Texnomic.ORMi;
using Texnomic.ORMi.Attributes;

namespace Texnomic.WMI.Network
{
    [WmiClass("MSFT_NetAdapter", @"root\StandardCimv2")]
    public class NetAdapter : WmiInstance
    {
        [WmiProperty("InterfaceGuid")]
        public string ID { get; set; }

        public string Name { get; set; }

        public bool Virtual { get; set; }

        public bool Physical { get; set; }
    }
}

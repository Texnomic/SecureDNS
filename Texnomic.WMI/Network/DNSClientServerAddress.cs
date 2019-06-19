using Texnomic.ORMi;
using Texnomic.ORMi.Attributes;

namespace Texnomic.WMI.Network
{
    [WmiClass("MSFT_DNSClientServerAddress", @"ROOT\StandardCimv2")]
    public class DNSClientServerAddress : WmiInstance
    {
        [WmiProperty("ElementName")]
        public string Name { get; set; }

        [WmiProperty("InterfaceIndex")]
        public uint Index { get; set; }

        [WmiProperty("InterfaceAlias")]
        public string Alias { get; set; }

    }
}

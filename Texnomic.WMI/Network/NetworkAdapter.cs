using Texnomic.ORMi;
using Texnomic.ORMi.Attributes;

namespace Texnomic.WMI.Network
{
    [WmiClass("Win32_NetworkAdapter", @"ROOT\CIMV2")]
    public class NetworkAdapter : WmiInstance
    {
        public uint Index { get; set; }

        [WmiProperty("GUID")]
        public string Guid { get; set; }

        [WmiProperty("Description")]
        public string Device { get; set; }

        [WmiProperty("NetConnectionID")]
        public string Name { get; set; }
    }
}

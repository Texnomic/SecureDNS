using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Texnomic.ORMi;
using Texnomic.ORMi.Attributes;

namespace Texnomic.WMI.Network
{
    [WmiClass("Win32_NetworkAdapterConfiguration", @"ROOT\CIMV2")]
    public class NetworkAdapterConfiguration : WmiInstance
    {
        public uint Index { get; set; }

        [WmiProperty("Description")]
        public string Name { get; set; }

        [WmiProperty("IPAddress")]
        public string[] IPAddress { get; set; }

        [WmiProperty("IPSubnet")]
        public string[] IPSubnet { get; set; }

        [WmiProperty("MACAddress")]
        public string MAC { get; set; }

        [WmiProperty("DNSServerSearchOrder")]
        public string[] DNS { get; set; }

        public uint SetDnsServers(params IPAddress[] Addresses)
        {
            var IPs = new string[Addresses.Length];

            for (int i = 0; i < Addresses.Length; i++)
            {
                IPs[i] = Addresses[i].ToString();
            }

            var Dictionary = new Dictionary<string, object>
            {
                { "DNSServerSearchOrder", IPs }
            };

            return Execute<uint>("SetDNSServerSearchOrder", Dictionary);
        }

        public void ResetDnsServers()
        {
            //Execute("RequestStateChange", 11);
        }
    }
}

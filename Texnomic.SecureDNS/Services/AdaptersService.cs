using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Models;

namespace Texnomic.SecureDNS.Services
{
    //+------------------------------------------------------------------------------+
    //|                    |   PlatformID    |   Major version   |   Minor version   |
    //+------------------------------------------------------------------------------+
    //| Windows 95         |  Win32Windows   |         4         |          0        |
    //| Windows 98         |  Win32Windows   |         4         |         10        |
    //| Windows Me         |  Win32Windows   |         4         |         90        |
    //| Windows NT 4.0     |  Win32NT        |         4         |          0        |
    //| Windows 2000       |  Win32NT        |         5         |          0        |
    //| Windows XP         |  Win32NT        |         5         |          1        |
    //| Windows 2003       |  Win32NT        |         5         |          2        |
    //| Windows Vista      |  Win32NT        |         6         |          0        |
    //| Windows 2008       |  Win32NT        |         6         |          0        |
    //| Windows 7          |  Win32NT        |         6         |          1        |
    //| Windows 2008 R2    |  Win32NT        |         6         |          1        |
    //| Windows 8          |  Win32NT        |         6         |          2        |
    //| Windows 8.1        |  Win32NT        |         6         |          3        |
    //+------------------------------------------------------------------------------+
    //| Windows 10         |  Win32NT        |        10         |          0        |
    //+------------------------------------------------------------------------------+


    public class AdaptersService
    {
        private readonly string[] Loopback = { "127.0.0.1", "127.0.0.1" };

        public List<Adapter> Adapters => GetAdapters();

        private List<Adapter> GetAdapters()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                                    .Select(NIC => new Adapter
                                    {
                                        ID = NIC.Id,
                                        Name = NIC.Name,
                                        Description = NIC.Description,
                                        Mac = NIC.GetPhysicalAddress().ToString(),
                                        DNS = string.Join(", ", NIC.GetIPProperties().DnsAddresses)
                                    })
                                    .ToList();
        }

        public async Task SetDNSAsync(Adapter Adapter)
        {
            try
            {
                await Task.Delay(1000);
                //
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public async Task ResetDNSAsync(Adapter Adapter)
        {
            try
            {
                await Task.Delay(1000);
                //
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }
    }
}

using System;
using System.Management;
using System.Management.Automation;
using System.Threading.Tasks;

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


    public class NetworkService
    {
        public async Task SetDNS()
        {
            using (var Shell = PowerShell.Create())
            {
                await Shell.AddScript("Get-NetAdapter -Physical | ForEach-Object { Set-DnsClientServerAddress $_.Name -ServerAddresses (\"127.0.0.1\") }")
                           .InvokeAsync();
            }
        }

        public async Task ResetDNS()
        {


            using (var Shell = PowerShell.Create())
            {
                await Shell.AddScript("Get-NetAdapter -Physical | ForEach-Object { Set-DnsClientServerAddress $_.Name -ResetServerAddresses }")
                           .InvokeAsync();
            }
        }

        private bool IsWindows7 => Environment.OSVersion.Version.Major == 6 & Environment.OSVersion.Version.Minor == 1;

    }
}

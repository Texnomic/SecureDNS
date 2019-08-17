using System;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Texnomic.SecureDNS.Automation.Models;

namespace Texnomic.SecureDNS.Automation
{
    public static class Network
    {
        public static class Adapters
        {
            public static async Task<Adapter[]> Get()
            {
                
                using var PSInstance = PowerShell.Create();

                await PSInstance.AddScript("Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process -Force").InvokeAsync();

                await PSInstance.AddScript("Import-Module NetAdapter").InvokeAsync();

                var PSDataCollection = await PSInstance.AddScript("Get-NetAdapter -Physical | ForEach-Object { @{ Index = $_.ifIndex; Name = $_.Name; Description = $_.InterfaceDescription; Status = $_.Status; Mac = $_.MacAddress; Speed = $_.LinkSpeed; IP = $(Get-NetIPAddress -InterfaceIndex $_.ifIndex | Select -ExpandProperty IPv4Address); DNS = $(Get-DnsClientServerAddress -AddressFamily IPv4 -InterfaceIndex $_.ifIndex | Select -ExpandProperty ServerAddresses) } } | ConvertTo-Json")
                                                       .InvokeAsync();

                var Json = PSDataCollection.FirstOrDefault()?.BaseObject as string;

                return JsonConvert.DeserializeObject<Adapter[]>(Json);
            }
        }


        public static class DNS
        {
            public static async Task<bool> Set(IPAddress[] IPAddresses)
            {
                try
                {
                    using var PSInstance = PowerShell.Create();

                    await PSInstance.AddScript("Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process -Force").InvokeAsync();

                    await PSInstance.AddScript("Import-Module NetAdapter, DnsClient").InvokeAsync();

                    var Result = await PSInstance.AddScript("Get-NetAdapter -Physical | ForEach-Object { Set-DnsClientServerAddress $_.Name -ServerAddresses (\"127.0.0.1\") }")
                                                 .InvokeAsync();

                    return !PSInstance.HadErrors;
                }
                catch (Exception Error)
                {
                    Console.WriteLine(Error.Message);
                    return false;
                }
            }

            public static async Task<bool> Reset()
            {
                try
                {
                    using var PSInstance = PowerShell.Create();

                    await PSInstance.AddScript("Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process -Force").InvokeAsync();

                    await PSInstance.AddScript("Import-Module NetAdapter, DnsClient").InvokeAsync();

                    var Result = await PSInstance.AddScript("Get-NetAdapter -Physical | ForEach-Object { Set-DnsClientServerAddress $_.Name -ResetServerAddresses }")
                                                 .InvokeAsync();

                    return !PSInstance.HadErrors;
                }
                catch (Exception Error)
                {
                    Console.WriteLine(Error.Message);
                    return false;
                }
            }
        }


    }
}

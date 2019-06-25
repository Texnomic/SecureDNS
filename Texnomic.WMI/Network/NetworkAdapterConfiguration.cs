using System.Collections.Generic;
using System.Linq;
using System.Net;
using Texnomic.ORMi;
using Texnomic.ORMi.Attributes;

namespace Texnomic.WMI.Network
{
    [WmiClass("Win32_NetworkAdapterConfiguration", @"ROOT\CIMV2")]
    public class NetworkAdapterConfiguration : WmiInstance
    {
        public uint Index { get; set; }

        [WmiProperty("Description")]
        public string Device { get; set; }

        [WmiProperty("IPAddress")]
        public string[] IPAddress { get; set; }

        [WmiProperty("IPSubnet")]
        public string[] IPSubnet { get; set; }

        [WmiProperty("MACAddress")]
        public string Mac { get; set; }

        [WmiProperty("DNSServerSearchOrder")]
        public string[] DNS { get; set; }

        public SetDnsServersResult SetDnsServers(params IPAddress[] Addresses)
        {
            var Dictionary = new Dictionary<string, string[]>
            {
                { "DNSServerSearchOrder", Addresses.Select(IP => IP.ToString()).ToArray() }
            };

            return Execute<string[], SetDnsServersResult>("SetDNSServerSearchOrder", Dictionary);
        }

        public void ResetDnsServers()
        {
            //Execute("RequestStateChange", 11);
        }

        public enum SetDnsServersResult : uint
        {
            SuccessfulCompletionNoRebootRequired = 0,

            SuccessfulCompletionRebootRequired = 1,

            MethodNotSupportedOnThisPlatform = 64,

            UnknownFailure = 65,

            InvalidSubnetMask = 66,

            AnErrorOccurredWhileProcessingAnInstanceThatWasReturned = 67,

            InvalidInputParameter = 68,

            MoreThan5GatewaysSpecified = 69,

            InvalidIpAddress = 70,

            InvalidGatewayIpAddress = 71,

            AnErrorOccurredWhileAccessingTheRegistryForTheRequestedInformation = 72,

            InvalidDomainName = 73,

            InvalidHostName = 74,

            NoPrimarysecondaryWinsServerDefined = 75,

            InvalidFile = 76,

            InvalidSystemPath = 77,

            FileCopyFailed = 78,

            InvalidSecurityParameter = 79,

            UnableToConfigureTcpipService = 80,

            UnableToConfigureDhcpService = 81,

            UnableToRenewDhcpLease = 82,

            UnableToReleaseDhcpLease = 83,

            IpNotEnabledOnAdapter = 84,

            IpxNotEnabledOnAdapter = 85,

            FramenetworkNumberBoundsError = 86,

            InvalidFrameType = 87,

            InvalidNetworkNumber = 88,

            DuplicateNetworkNumber = 89,

            ParameterOutOfBounds = 90,

            AccessDenied = 91,

            OutOfMemory = 92,

            AlreadyExists = 93,

            PathFileOrObjectNotFound = 94,

            UnableToNotifyService = 95,

            UnableToNotifyDnsService = 96,

            InterfaceNotConfigurable = 97,

            NotAllDhcpLeasesCouldBeReleasedrenewed = 98,

            DhcpNotEnabledOnAdapter = 10,
        }
    }
}

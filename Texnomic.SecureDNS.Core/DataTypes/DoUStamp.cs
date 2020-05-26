using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#plain-dns-stamps"/>
    /// </summary>
    public class DoUStamp : IStamp
    {
        public bool DnsSec { get; set; }

        public bool NoLog { get; set; }

        public bool NoFilter { get; set; }

        public byte[] Flags { get; set; }

        public PrefixedString Address { get; set; }
    }
}

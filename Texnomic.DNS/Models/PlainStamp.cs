using BinarySerialization;

namespace Texnomic.DNS.Models
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#plain-dns-stamps"/>
    /// </summary>
    public class PlainStamp
    {
        [FieldOrder(0), FieldBitLength(8)] 
        public bool DnsSec { get; set; }

        [FieldOrder(1), FieldBitLength(8)] 
        public bool NoLog { get; set; }

        [FieldOrder(2), FieldBitLength(8)] 
        public bool NoFilter { get; set; }

        [FieldOrder(3), FieldBitLength(40)] 
        public byte[] Flags { get; set; }

        [FieldOrder(4)] 
        public PrefixedString Address { get; set; }
    }
}

namespace Texnomic.SecureDNS.Abstractions.Enums
{
    public enum OptionCode : ushort
    {
        Reserved = 0,
        LLQ = 1,
        UL = 2,
        NSID = 3,
        DAU = 5,
        DHU = 6,
        N3U = 7,
        EDNS_ClientSubnet = 8,
        EDNS_Expire = 9,
        COOKIE = 10,
        EDNS_TcpKeepAlive = 11,
        Padding = 12,
        CHAIN = 13,
        EDNS_KeyTag = 14,
        Unassigned = 15,
        EDNS_ClientTag = 16,
        EDNS_ServerTag = 17,
        DeviceID = 26946,
    }
}

namespace Texnomic.Socks5.Enum
{
    internal enum Command : byte
    {
        Query = 0,
        Connect = 1,
        Bind = 2,
        UdpAssociate = 3,
        Resolve = 240,
        ResolvePTR = 241,

    }
}

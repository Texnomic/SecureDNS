namespace Texnomic.SecureDNS.Abstractions.Enums;

/// <summary>
/// Address Family enumeration for EDNS Client Subnet (RFC 7871)
/// https://www.iana.org/assignments/address-family-numbers/address-family-numbers.xhtml
/// </summary>
public enum AddressFamily : ushort
{
    /// <summary>
    /// IPv4 Address Family
    /// </summary>
    IPv4 = 1,

    /// <summary>
    /// IPv6 Address Family
    /// </summary>
    IPv6 = 2,
}

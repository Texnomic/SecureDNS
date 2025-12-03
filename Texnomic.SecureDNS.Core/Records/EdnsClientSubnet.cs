using System.Net;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.Records;

//                     1  1  1  1  1  1
//   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
// +---+---+---+---+---+---+---+---+---+---+---+---+
// |                  FAMILY                       |
// +---+---+---+---+---+---+---+---+---+---+---+---+
// |     SOURCE PREFIX-LENGTH      | SCOPE PREFIX- |
// +---+---+---+---+---+---+---+---+---+---+---+---+
// |     LENGTH (OPTIONAL)         |               |
// +---+---+---+---+---+---+---+---+               +
// |                  ADDRESS...                   /
// +---+---+---+---+---+---+---+---+---+---+---+---+

/// <summary>
/// EDNS Client Subnet Option Record <see href="https://tools.ietf.org/html/rfc7871">(RFC 7871)</see>
/// </summary>
public class EdnsClientSubnet : IRecord
{
    /// <summary>
    /// Address family (IPv4 = 1, IPv6 = 2)
    /// </summary>
    public AddressFamily Family { get; set; }

    /// <summary>
    /// Number of significant bits in the ADDRESS field
    /// </summary>
    public byte SourcePrefixLength { get; set; }

    /// <summary>
    /// Number of significant bits the authoritative server used
    /// </summary>
    public byte ScopePrefixLength { get; set; }

    /// <summary>
    /// Client subnet address (leftmost significant bits)
    /// </summary>
    public IPAddress Address { get; set; }
}

using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Records;
//                                 1  1  1  1  1  1
//   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5

/// <summary>
/// Service Resource Record <see href="https://tools.ietf.org/html/rfc2052">(SRV)</see>
/// </summary>
public class SRV : IRecord
{
    public ushort Priority { get; set; }

    public ushort Weight { get; set; }

    public ushort Port { get; set; }

    public IDomain Target { get; set; }
}
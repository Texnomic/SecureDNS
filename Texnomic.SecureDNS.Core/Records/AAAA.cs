using System.Net;
using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Records;
//                                 1  1  1  1  1  1
//   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// |                    ADDRESS                    |
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

/// <summary>
/// 128-bit Internet Address Resource Record <see href="https://tools.ietf.org/html/rfc3596">(AAAA)</see>
/// </summary>
public class AAAA : IRecord
{
    [LogAsScalar(true)]
    public IPAddress Address { get; set; }
}
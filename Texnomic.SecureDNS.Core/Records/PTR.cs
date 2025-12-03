using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Records;
//                                 1  1  1  1  1  1
//   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// /                   PTRDNAME                     /
// /                                               /
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

/// <summary>
/// Pointer Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.12">(PTR)</see>
/// </summary>
public class PTR : IRecord
{
    [LogAsScalar(true)]
    public IDomain Domain { get; set; }
}
using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Records;
//                     1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |            Type               |            Key Tag            |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// | Algorithm     |                                               /
// +---------------+        Certificate Data                       /
// /                                                               /
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|

/// <summary>
/// Certificate Resource Record <see href="https://tools.ietf.org/id/draft-hallambaker-certhash-00.html#rfc.section.3.1">(CERT)</see>
/// </summary>
[LogAsScalar(true)]
public class CERT : IRecord
{

}
using System;
using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Records;
//                                 1  1  1  1  1  1
//   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// /                     MNAME                     /
// /                                               /
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// /                     RNAME                     /
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// |                    SERIAL                     |
// |                                               |
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// |                    REFRESH                    |
// |                                               |
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// |                     RETRY                     |
// |                                               |
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// |                    EXPIRE                     |
// |                                               |
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
// |                    MINIMUM                    |
// |                                               |
// +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

/// <summary>
/// Start Of Authority Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.13">(SOA)</see>
/// </summary>
public class SOA : IRecord
{
    [LogAsScalar(true)]
    public IDomain PrimaryNameServer { get; set; }

    [LogAsScalar(true)]
    public IDomain ResponsibleAuthorityMailbox { get; set; }

    public uint SerialNumber { get; set; }

    [LogAsScalar(true)]
    public TimeSpan RefreshInterval { get; set; }

    [LogAsScalar(true)]
    public TimeSpan RetryInterval { get; set; }

    [LogAsScalar(true)]
    public TimeSpan ExpiryLimit { get; set; }

    [LogAsScalar(true)]
    public TimeSpan TimeToLive { get; set; }
}
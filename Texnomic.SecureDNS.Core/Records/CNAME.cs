using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                     CNAME                     /
    // /                                               /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Canonical Name Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.1">(CName)</see>
    /// </summary>
    public class CNAME : IRecord
    {
        public IDomain Domain { get; set; }
    }
}

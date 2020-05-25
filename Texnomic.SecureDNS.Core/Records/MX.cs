using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                  PREFERENCE                   |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                   EXCHANGE                    /
    // /                                               /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Mail Exchange Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.9">(MX)</see>
    /// </summary>
    public class MX : IRecord
    {
        public short Preference { get; set; }

        public IDomain Exchange { get; set; }
    }
}

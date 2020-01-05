using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Factories;
using Destructurama.Attributed;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    ADDRESS                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// 32-bit Internet Address Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.4.1">(A)</see>
    /// </summary>
    public class A : IRecord
    {
        [FieldOrder(0)]
        [LogAsScalar(true)]
        [SubtypeFactory(nameof(Address), typeof(IPv4AddressFactory), BindingMode = BindingMode.OneWayToSource)]
        public IIPv4Address Address { get; set; }
    }
}

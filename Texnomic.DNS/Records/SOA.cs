using BinarySerialization;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Factories;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
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
        [FieldOrder(0)]
        [LogAsScalar(true)]
        public Domain PrimaryNameServer { get; set; }

        [FieldOrder(1)]
        [LogAsScalar(true)]
        public Domain ResponsibleAuthorityMailbox { get; set; }

        [FieldOrder(2), FieldEndianness(Endianness.Big)]
        public uint SerialNumber { get; set; }

        [FieldOrder(3), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(TimeToLive), typeof(TimeToLiveFactory), BindingMode = BindingMode.OneWayToSource)]
        [LogAsScalar(true)]
        public ITimeToLive RefreshInterval { get; set; }

        [FieldOrder(4), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(TimeToLive), typeof(TimeToLiveFactory), BindingMode = BindingMode.OneWayToSource)]
        [LogAsScalar(true)]
        public ITimeToLive RetryInterval { get; set; }

        [FieldOrder(5), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(TimeToLive), typeof(TimeToLiveFactory), BindingMode = BindingMode.OneWayToSource)]
        [LogAsScalar(true)]
        public ITimeToLive ExpiryLimit { get; set; }

        [FieldOrder(6), FieldEndianness(Endianness.Big)]
        [SubtypeFactory(nameof(TimeToLive), typeof(TimeToLiveFactory), BindingMode = BindingMode.OneWayToSource)]
        [LogAsScalar(true)]
        public ITimeToLive TimeToLive { get; set; }
    }
}

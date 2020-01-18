using BinarySerialization;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Abstractions
{
    public interface IQuestion
    {
        [FieldOrder(0)]
        IDomain Domain { get; set; }

        [FieldOrder(1)]
        RecordType Type { get; set; }

        [FieldOrder(2)]
        RecordClass Class { get; set; }
    }
}

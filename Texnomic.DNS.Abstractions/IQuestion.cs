using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Abstractions
{
    public interface IQuestion
    {
        IDomain Domain { get; set; }

        string Name { get; set; }

        RecordType Type { get; set; }

        RecordClass Class { get; set; }
    }
}

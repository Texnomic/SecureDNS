using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Abstractions;

public interface IQuestion
{
    IDomain Domain { get; set; }

    RecordType Type { get; set; }

    RecordClass? Class { get; set; }
}
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.DataTypes;

public class Question : IQuestion
{
    public IDomain Domain { get; set; }

    public RecordType Type { get; set; }

    public RecordClass? Class { get; set; }
}
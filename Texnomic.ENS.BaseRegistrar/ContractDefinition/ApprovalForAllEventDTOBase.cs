using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Event("ApprovalForAll")]
public class ApprovalForAllEventDtoBase : IEventDTO
{
    [Parameter("address", "owner", 1, true)]
    public virtual string Owner { get; set; }
    [Parameter("address", "operator", 2, true)]
    public virtual string Operator { get; set; }
    [Parameter("bool", "approved", 3, false)]
    public virtual bool Approved { get; set; }
}
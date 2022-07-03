using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Function("isApprovedForAll", "bool")]
public class IsApprovedForAllFunctionBase : FunctionMessage
{
    [Parameter("address", "owner", 1)]
    public virtual string Owner { get; set; }
    [Parameter("address", "operator", 2)]
    public virtual string Operator { get; set; }
}
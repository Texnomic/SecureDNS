using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Function("balanceOf", "uint256")]
public class BalanceOfFunctionBase : FunctionMessage
{
    [Parameter("address", "owner", 1)]
    public virtual string Owner { get; set; }
}
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Function("transferOwnership")]
public class TransferOwnershipFunctionBase : FunctionMessage
{
    [Parameter("address", "newOwner", 1)]
    public virtual string NewOwner { get; set; }
}
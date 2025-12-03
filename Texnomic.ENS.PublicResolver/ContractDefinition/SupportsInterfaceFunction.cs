using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class SupportsInterfaceFunction : SupportsInterfaceFunctionBase { }

[Function("supportsInterface", "bool")]
public class SupportsInterfaceFunctionBase : FunctionMessage
{
    [Parameter("bytes4", "interfaceID", 1)]
    public virtual byte[] InterfaceID { get; set; }
}
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class SetInterfaceFunction : SetInterfaceFunctionBase { }

[Function("setInterface")]
public class SetInterfaceFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "node", 1)]
    public virtual byte[] Node { get; set; }
    [Parameter("bytes4", "interfaceID", 2)]
    public virtual byte[] InterfaceID { get; set; }
    [Parameter("address", "implementer", 3)]
    public virtual string Implementer { get; set; }
}
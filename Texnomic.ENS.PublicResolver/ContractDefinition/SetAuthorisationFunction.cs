using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class SetAuthorisationFunction : SetAuthorisationFunctionBase { }

[Function("setAuthorisation")]
public class SetAuthorisationFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "node", 1)]
    public virtual byte[] Node { get; set; }
    [Parameter("address", "target", 2)]
    public virtual string Target { get; set; }
    [Parameter("bool", "isAuthorised", 3)]
    public virtual bool IsAuthorised { get; set; }
}
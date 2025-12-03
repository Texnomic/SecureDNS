using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class SetPublicKeyFunction : SetPublicKeyFunctionBase { }

[Function("setPublicKey")]
public class SetPublicKeyFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "node", 1)]
    public virtual byte[] Node { get; set; }
    [Parameter("bytes32", "x", 2)]
    public virtual byte[] X { get; set; }
    [Parameter("bytes32", "y", 3)]
    public virtual byte[] Y { get; set; }
}
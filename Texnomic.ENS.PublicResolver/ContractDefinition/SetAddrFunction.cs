using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class SetAddrFunction : SetAddrFunctionBase { }

[Function("setAddr")]
public class SetAddrFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "node", 1)]
    public virtual byte[] Node { get; set; }
    [Parameter("address", "addr", 2)]
    public virtual string Addr { get; set; }
}
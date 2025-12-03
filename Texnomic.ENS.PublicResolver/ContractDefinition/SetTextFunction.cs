using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class SetTextFunction : SetTextFunctionBase { }

[Function("setText")]
public class SetTextFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "node", 1)]
    public virtual byte[] Node { get; set; }
    [Parameter("string", "key", 2)]
    public virtual string Key { get; set; }
    [Parameter("string", "value", 3)]
    public virtual string Value { get; set; }
}
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class AuthorisationsFunction : AuthorisationsFunctionBase { }

[Function("authorisations", "bool")]
public class AuthorisationsFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "", 1)]
    public virtual byte[] ReturnValue1 { get; set; }
    [Parameter("address", "", 2)]
    public virtual string ReturnValue2 { get; set; }
    [Parameter("address", "", 3)]
    public virtual string ReturnValue3 { get; set; }
}
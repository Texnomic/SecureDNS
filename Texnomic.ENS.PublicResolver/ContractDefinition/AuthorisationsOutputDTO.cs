using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class AuthorisationsOutputDto : AuthorisationsOutputDtoBase { }

[FunctionOutput]
public class AuthorisationsOutputDtoBase : IFunctionOutputDTO 
{
    [Parameter("bool", "", 1)]
    public virtual bool ReturnValue1 { get; set; }
}
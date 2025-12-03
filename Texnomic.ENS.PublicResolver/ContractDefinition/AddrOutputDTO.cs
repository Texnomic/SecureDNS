using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class AddrOutputDto : AddrOutputDtoBase { }

[FunctionOutput]
public class AddrOutputDtoBase : IFunctionOutputDTO 
{
    [Parameter("address", "", 1)]
    public virtual string ReturnValue1 { get; set; }
}
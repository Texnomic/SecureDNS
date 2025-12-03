using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class TextOutputDto : TextOutputDtoBase { }

[FunctionOutput]
public class TextOutputDtoBase : IFunctionOutputDTO 
{
    [Parameter("string", "", 1)]
    public virtual string ReturnValue1 { get; set; }
}
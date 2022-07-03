using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class InterfaceImplementerOutputDto : InterfaceImplementerOutputDtoBase { }

[FunctionOutput]
public class InterfaceImplementerOutputDtoBase : IFunctionOutputDTO 
{
    [Parameter("address", "", 1)]
    public virtual string ReturnValue1 { get; set; }
}
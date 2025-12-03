using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class PublicKeyOutputDto : PublicKeyOutputDtoBase { }

[FunctionOutput]
public class PublicKeyOutputDtoBase : IFunctionOutputDTO 
{
    [Parameter("bytes32", "x", 1)]
    public virtual byte[] X { get; set; }
    [Parameter("bytes32", "y", 2)]
    public virtual byte[] Y { get; set; }
}
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class AbiOutputDto : AbiOutputDtoBase { }

[FunctionOutput]
public class AbiOutputDtoBase : IFunctionOutputDTO 
{
    [Parameter("uint256", "", 1)]
    public virtual BigInteger ReturnValue1 { get; set; }
    [Parameter("bytes", "", 2)]
    public virtual byte[] ReturnValue2 { get; set; }
}
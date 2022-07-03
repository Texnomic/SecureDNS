using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[FunctionOutput]
public class MigrationLockPeriodOutputDtoBase : IFunctionOutputDTO
{
    [Parameter("uint256", "", 1)]
    public virtual BigInteger ReturnValue1 { get; set; }
}
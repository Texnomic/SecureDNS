using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [FunctionOutput]
    public class BaseNodeOutputDtoBase : IFunctionOutputDTO
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }
}
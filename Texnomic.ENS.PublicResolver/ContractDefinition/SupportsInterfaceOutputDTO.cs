using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class SupportsInterfaceOutputDto : SupportsInterfaceOutputDtoBase { }

    [FunctionOutput]
    public class SupportsInterfaceOutputDtoBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }
}

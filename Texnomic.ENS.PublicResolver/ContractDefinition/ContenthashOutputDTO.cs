using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class ContenthashOutputDto : ContenthashOutputDtoBase { }

    [FunctionOutput]
    public class ContenthashOutputDtoBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }
}

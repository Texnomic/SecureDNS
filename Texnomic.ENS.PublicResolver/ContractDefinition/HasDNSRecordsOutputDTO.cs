using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class HasDNSRecordsOutputDto : HasDNSRecordsOutputDtoBase { }

    [FunctionOutput]
    public class HasDNSRecordsOutputDtoBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }
}

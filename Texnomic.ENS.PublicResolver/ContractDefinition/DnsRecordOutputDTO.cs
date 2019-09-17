using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class DnsRecordOutputDto : DnsRecordOutputDtoBase { }

    [FunctionOutput]
    public class DnsRecordOutputDtoBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }
}

using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class DNSRecordChangedEventDto : DNSRecordChangedEventDtoBase { }

    [Event("DNSRecordChanged")]
    public class DNSRecordChangedEventDtoBase : IEventDTO
    {
        [Parameter("bytes32", "node", 1, true )]
        public virtual byte[] Node { get; set; }
        [Parameter("bytes", "name", 2, false )]
        public virtual byte[] Name { get; set; }
        [Parameter("uint16", "resource", 3, false )]
        public virtual ushort Resource { get; set; }
        [Parameter("bytes", "record", 4, false )]
        public virtual byte[] Record { get; set; }
    }
}

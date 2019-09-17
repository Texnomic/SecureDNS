using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class AddrChangedEventDto : AddrChangedEventDtoBase { }

    [Event("AddrChanged")]
    public class AddrChangedEventDtoBase : IEventDTO
    {
        [Parameter("bytes32", "node", 1, true )]
        public virtual byte[] Node { get; set; }
        [Parameter("address", "a", 2, false )]
        public virtual string A { get; set; }
    }
}

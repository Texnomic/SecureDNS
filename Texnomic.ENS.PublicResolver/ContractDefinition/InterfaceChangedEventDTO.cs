using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class InterfaceChangedEventDto : InterfaceChangedEventDtoBase { }

    [Event("InterfaceChanged")]
    public class InterfaceChangedEventDtoBase : IEventDTO
    {
        [Parameter("bytes32", "node", 1, true )]
        public virtual byte[] Node { get; set; }
        [Parameter("bytes4", "interfaceID", 2, true )]
        public virtual byte[] InterfaceID { get; set; }
        [Parameter("address", "implementer", 3, false )]
        public virtual string Implementer { get; set; }
    }
}

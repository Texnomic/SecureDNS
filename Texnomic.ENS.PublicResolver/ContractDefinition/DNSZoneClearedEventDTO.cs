using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class DNSZoneClearedEventDto : DNSZoneClearedEventDtoBase { }

[Event("DNSZoneCleared")]
public class DNSZoneClearedEventDtoBase : IEventDTO
{
    [Parameter("bytes32", "node", 1, true )]
    public virtual byte[] Node { get; set; }
}
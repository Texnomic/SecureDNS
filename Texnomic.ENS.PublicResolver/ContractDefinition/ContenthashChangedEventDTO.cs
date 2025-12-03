using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class ContenthashChangedEventDto : ContenthashChangedEventDtoBase { }

[Event("ContenthashChanged")]
public class ContenthashChangedEventDtoBase : IEventDTO
{
    [Parameter("bytes32", "node", 1, true )]
    public virtual byte[] Node { get; set; }
    [Parameter("bytes", "hash", 2, false )]
    public virtual byte[] Hash { get; set; }
}
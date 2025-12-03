using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class NameChangedEventDto : NameChangedEventDtoBase { }

[Event("NameChanged")]
public class NameChangedEventDtoBase : IEventDTO
{
    [Parameter("bytes32", "node", 1, true )]
    public virtual byte[] Node { get; set; }
    [Parameter("string", "name", 2, false )]
    public virtual string Name { get; set; }
}
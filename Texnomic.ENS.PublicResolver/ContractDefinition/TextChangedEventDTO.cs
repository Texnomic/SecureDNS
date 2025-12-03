using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class TextChangedEventDto : TextChangedEventDtoBase { }

[Event("TextChanged")]
public class TextChangedEventDtoBase : IEventDTO
{
    [Parameter("bytes32", "node", 1, true )]
    public virtual byte[] Node { get; set; }
    [Parameter("string", "indexedKey", 2, true )]
    public virtual string IndexedKey { get; set; }
    [Parameter("string", "key", 3, false )]
    public virtual string Key { get; set; }
}
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class PublicKeyChangedEventDto : PublicKeyChangedEventDtoBase { }

[Event("PublicKeyChanged")]
public class PublicKeyChangedEventDtoBase : IEventDTO
{
    [Parameter("bytes32", "node", 1, true )]
    public virtual byte[] Node { get; set; }
    [Parameter("bytes32", "x", 2, false )]
    public virtual byte[] X { get; set; }
    [Parameter("bytes32", "y", 3, false )]
    public virtual byte[] Y { get; set; }
}
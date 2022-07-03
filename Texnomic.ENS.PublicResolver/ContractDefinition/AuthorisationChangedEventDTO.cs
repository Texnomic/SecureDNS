using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class AuthorisationChangedEventDto : AuthorisationChangedEventDtoBase { }

[Event("AuthorisationChanged")]
public class AuthorisationChangedEventDtoBase : IEventDTO
{
    [Parameter("bytes32", "node", 1, true )]
    public virtual byte[] Node { get; set; }
    [Parameter("address", "owner", 2, true )]
    public virtual string Owner { get; set; }
    [Parameter("address", "target", 3, true )]
    public virtual string Target { get; set; }
    [Parameter("bool", "isAuthorised", 4, false )]
    public virtual bool IsAuthorised { get; set; }
}
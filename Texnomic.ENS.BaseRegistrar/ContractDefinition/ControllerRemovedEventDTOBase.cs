using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Event("ControllerRemoved")]
public class ControllerRemovedEventDtoBase : IEventDTO
{
    [Parameter("address", "controller", 1, true)]
    public virtual string Controller { get; set; }
}
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Event("ControllerAdded")]
    public class ControllerAddedEventDtoBase : IEventDTO
    {
        [Parameter("address", "controller", 1, true)]
        public virtual string Controller { get; set; }
    }
}
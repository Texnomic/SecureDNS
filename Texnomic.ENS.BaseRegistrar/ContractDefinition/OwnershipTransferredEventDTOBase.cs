using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Event("OwnershipTransferred")]
    public class OwnershipTransferredEventDtoBase : IEventDTO
    {
        [Parameter("address", "previousOwner", 1, true)]
        public virtual string PreviousOwner { get; set; }
        [Parameter("address", "newOwner", 2, true)]
        public virtual string NewOwner { get; set; }
    }
}
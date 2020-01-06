using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Event("Approval")]
    public class ApprovalEventDtoBase : IEventDTO
    {
        [Parameter("address", "owner", 1, true)]
        public virtual string Owner { get; set; }
        [Parameter("address", "approved", 2, true)]
        public virtual string Approved { get; set; }
        [Parameter("uint256", "tokenId", 3, true)]
        public virtual BigInteger TokenId { get; set; }
    }
}
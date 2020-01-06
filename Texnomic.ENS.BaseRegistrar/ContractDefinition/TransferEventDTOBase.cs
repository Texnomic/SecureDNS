using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Event("Transfer")]
    public class TransferEventDtoBase : IEventDTO
    {
        [Parameter("address", "from", 1, true)]
        public virtual string From { get; set; }
        [Parameter("address", "to", 2, true)]
        public virtual string To { get; set; }
        [Parameter("uint256", "tokenId", 3, true)]
        public virtual BigInteger TokenId { get; set; }
    }
}
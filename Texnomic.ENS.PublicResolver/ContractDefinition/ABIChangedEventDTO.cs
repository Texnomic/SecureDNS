using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class AbiChangedEventDto : AbiChangedEventDtoBase { }

    [Event("ABIChanged")]
    public class AbiChangedEventDtoBase : IEventDTO
    {
        [Parameter("bytes32", "node", 1, true )]
        public virtual byte[] Node { get; set; }
        [Parameter("uint256", "contentType", 2, true )]
        public virtual BigInteger ContentType { get; set; }
    }
}

using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Event("NameRenewed")]
    public class NameRenewedEventDtoBase : IEventDTO
    {
        [Parameter("uint256", "id", 1, true)]
        public virtual BigInteger Id { get; set; }
        [Parameter("uint256", "expires", 2, false)]
        public virtual BigInteger Expires { get; set; }
    }
}
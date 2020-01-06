using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Event("NameMigrated")]
    public class NameMigratedEventDtoBase : IEventDTO
    {
        [Parameter("uint256", "id", 1, true)]
        public virtual BigInteger Id { get; set; }
        [Parameter("address", "owner", 2, true)]
        public virtual string Owner { get; set; }
        [Parameter("uint256", "expires", 3, false)]
        public virtual BigInteger Expires { get; set; }
    }
}
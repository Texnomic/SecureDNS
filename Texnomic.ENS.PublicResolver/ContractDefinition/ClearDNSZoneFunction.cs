using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class ClearDNSZoneFunction : ClearDNSZoneFunctionBase { }

    [Function("clearDNSZone")]
    public class ClearDNSZoneFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "node", 1)]
        public virtual byte[] Node { get; set; }
    }
}

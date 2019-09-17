using System.Numerics;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class AbiFunction : AbiFunctionBase { }

    [Function("ABI", typeof(AbiOutputDto))]
    public class AbiFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "node", 1)]
        public virtual byte[] Node { get; set; }
        [Parameter("uint256", "contentTypes", 2)]
        public virtual BigInteger ContentTypes { get; set; }
    }
}

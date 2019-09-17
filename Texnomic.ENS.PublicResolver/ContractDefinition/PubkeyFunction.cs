using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class PublicKeyFunction : PublicKeyFunctionBase { }

    [Function("PublicKey", typeof(PublicKeyOutputDto))]
    public class PublicKeyFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "node", 1)]
        public virtual byte[] Node { get; set; }
    }
}

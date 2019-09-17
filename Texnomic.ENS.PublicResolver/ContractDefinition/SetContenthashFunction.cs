using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class SetContenthashFunction : SetContenthashFunctionBase { }

    [Function("setContenthash")]
    public class SetContenthashFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "node", 1)]
        public virtual byte[] Node { get; set; }
        [Parameter("bytes", "hash", 2)]
        public virtual byte[] Hash { get; set; }
    }
}

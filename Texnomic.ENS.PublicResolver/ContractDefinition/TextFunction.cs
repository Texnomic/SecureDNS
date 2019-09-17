using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class TextFunction : TextFunctionBase { }

    [Function("text", "string")]
    public class TextFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "node", 1)]
        public virtual byte[] Node { get; set; }
        [Parameter("string", "key", 2)]
        public virtual string Key { get; set; }
    }
}

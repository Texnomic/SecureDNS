using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class SetNameFunction : SetNameFunctionBase { }

    [Function("setName")]
    public class SetNameFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "node", 1)]
        public virtual byte[] Node { get; set; }
        [Parameter("string", "name", 2)]
        public virtual string Name { get; set; }
    }
}

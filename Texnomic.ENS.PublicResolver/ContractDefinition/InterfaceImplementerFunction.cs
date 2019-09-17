using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition
{
    public class InterfaceImplementerFunction : InterfaceImplementerFunctionBase { }

    [Function("interfaceImplementer", "address")]
    public class InterfaceImplementerFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "node", 1)]
        public virtual byte[] Node { get; set; }
        [Parameter("bytes4", "interfaceID", 2)]
        public virtual byte[] InterfaceID { get; set; }
    }
}

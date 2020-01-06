using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Function("removeController")]
    public class RemoveControllerFunctionBase : FunctionMessage
    {
        [Parameter("address", "controller", 1)]
        public virtual string Controller { get; set; }
    }
}
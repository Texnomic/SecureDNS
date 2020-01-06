using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Function("addController")]
    public class AddControllerFunctionBase : FunctionMessage
    {
        [Parameter("address", "controller", 1)]
        public virtual string Controller { get; set; }
    }
}
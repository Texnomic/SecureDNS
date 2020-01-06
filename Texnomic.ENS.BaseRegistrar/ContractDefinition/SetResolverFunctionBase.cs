using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Function("setResolver")]
    public class SetResolverFunctionBase : FunctionMessage
    {
        [Parameter("address", "resolver", 1)]
        public virtual string Resolver { get; set; }
    }
}
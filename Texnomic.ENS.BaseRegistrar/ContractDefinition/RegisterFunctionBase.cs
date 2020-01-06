using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Function("register", "uint256")]
    public class RegisterFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "id", 1)]
        public virtual BigInteger Id { get; set; }
        [Parameter("address", "owner", 2)]
        public virtual string Owner { get; set; }
        [Parameter("uint256", "duration", 3)]
        public virtual BigInteger Duration { get; set; }
    }
}
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Function("approve")]
public class ApproveFunctionBase : FunctionMessage
{
    [Parameter("address", "to", 1)]
    public virtual string To { get; set; }
    [Parameter("uint256", "tokenId", 2)]
    public virtual BigInteger TokenId { get; set; }
}
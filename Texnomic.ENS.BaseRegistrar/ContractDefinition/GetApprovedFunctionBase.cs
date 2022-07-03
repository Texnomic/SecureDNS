using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Function("getApproved", "address")]
public class GetApprovedFunctionBase : FunctionMessage
{
    [Parameter("uint256", "tokenId", 1)]
    public virtual BigInteger TokenId { get; set; }
}
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Function("nameExpires", "uint256")]
public class NameExpiresFunctionBase : FunctionMessage
{
    [Parameter("uint256", "id", 1)]
    public virtual BigInteger Id { get; set; }
}
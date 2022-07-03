using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition;

[Function("previousRegistrar", "address")]
public class PreviousRegistrarFunctionBase : FunctionMessage
{

}
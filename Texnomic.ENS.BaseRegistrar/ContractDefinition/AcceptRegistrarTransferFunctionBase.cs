using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Texnomic.ENS.BaseRegistrar.ContractDefinition
{
    [Function("acceptRegistrarTransfer")]
    public class AcceptRegistrarTransferFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "label", 1)]
        public virtual byte[] Label { get; set; }
        [Parameter("address", "deed", 2)]
        public virtual string Deed { get; set; }
        [Parameter("uint256", "", 3)]
        public virtual BigInteger ReturnValue3 { get; set; }
    }
}
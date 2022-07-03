using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class HasDNSRecordsFunction : HasDNSRecordsFunctionBase { }

[Function("hasDNSRecords", "bool")]
public class HasDNSRecordsFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "node", 1)]
    public virtual byte[] Node { get; set; }
    [Parameter("bytes32", "name", 2)]
    public virtual byte[] Name { get; set; }
}
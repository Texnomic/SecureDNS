using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Texnomic.ENS.PublicResolver.ContractDefinition;

public class DnsRecordFunction : DnsRecordFunctionBase { }

[Function("dnsRecord", "bytes")]
public class DnsRecordFunctionBase : FunctionMessage
{
    [Parameter("bytes32", "node", 1)]
    public virtual byte[] Node { get; set; }
    [Parameter("bytes32", "name", 2)]
    public virtual byte[] Name { get; set; }
    [Parameter("uint16", "resource", 3)]
    public virtual ushort Resource { get; set; }
}
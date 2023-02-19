using System.Net;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core.Records;

namespace Texnomic.SecureDNS.Data.Models;

public class Query
{
    public IPEndPoint IPEndPoint { get; set; }
    public IMessage Request { get; set; }
    public IMessage Response { get; set; }

    public string IPAddress => IPEndPoint.Address.ToString();
    public string Port => IPEndPoint.Port.ToString();
    public string Domain => Request.Questions.First().Domain.ToString();
    public string Resolve => Response.Answers
        .Where(Record => Record.Type == RecordType.A || Record.Type == RecordType.PTR)
        .Cast<A>()
        .Select(Record => Record.Address)
        .First()
        .ToString();
}
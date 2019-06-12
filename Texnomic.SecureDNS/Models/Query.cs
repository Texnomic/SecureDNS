using System.Linq;
using System.Net;
using Texnomic.DNS.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Records;

namespace Texnomic.SecureDNS.Models
{
    public class Query
    {
        public IPEndPoint IPEndPoint { get; set; }
        public Message Request { get; set; }
        public Message Response { get; set; }

        public string IPAddress => IPEndPoint.Address.ToString();
        public string Port => IPEndPoint.Port.ToString();
        public string Domain => Request.Questions.First().Domain.ToString();
        public string Resolve => Response.Answers
                                         .Where(Record => Record.Type == RecordType.A || Record.Type == RecordType.PTR)
                                         .Cast<A>()
                                         .Select(Record => Record.IPAddress)
                                         .First()
                                         .ToString();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Texnomic.DNS.Protocol;
using Texnomic.DNS.Protocol.ResourceRecords;

namespace Texnomic.SecureDNS.Models
{
    public class Query
    {
        public IPEndPoint IPEndPoint { get; set; }
        public IRequest Request { get; set; }
        public IResponse Response { get; set; }

        public string IPAddress => IPEndPoint.Address.ToString();
        public string Port => IPEndPoint.Port.ToString();
        public string Domain => Request.Questions[0].Name.ToString();
        public string Resolve => Response.AnswerRecords
                                         .Where(Record => Record.Type == RecordType.A || Record.Type == RecordType.PTR)
                                         .Cast<IPAddressResourceRecord>()
                                         .Select(Record => Record.IPAddress)
                                         .ToList()[0]
                                         .ToString();
    }
}

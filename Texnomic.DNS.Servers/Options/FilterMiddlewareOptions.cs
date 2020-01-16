using Serilog;
using System.Collections.Generic;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.FilterLists.Enums;

namespace Texnomic.DNS.Servers.Options
{
    public class FilterMiddlewareOptions
    {
        public List<Tags> ListTags { get; set; } = new List<Tags> { Tags.Malware, Tags.Phishing, Tags.Crypto };

        public IMessage Template { get; set; }  = new Message()
        {
            MessageType = MessageType.Response,
            ResponseCode = ResponseCode.Refused,
        };
    }
}

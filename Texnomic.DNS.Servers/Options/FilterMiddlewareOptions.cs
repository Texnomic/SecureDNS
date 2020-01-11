using Serilog;
using System.Collections.Generic;
using Texnomic.FilterLists.Enums;

namespace Texnomic.DNS.Servers.Options
{
    public class FilterMiddlewareOptions
    {
        public List<Tags> ListTags { get; set; } = new List<Tags> { Tags.Malware, Tags.Phishing, Tags.Crypto };
    }
}

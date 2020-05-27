using System.Collections.Generic;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.DNS.Servers.Options
{
    public class MasterFileMiddlewareOptions
    {
        public List<IAnswer> Answers { get; set; }
    }
}

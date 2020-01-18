using System;
using System.Collections.Generic;
using System.Text;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Servers.Options
{
    public class MasterFileMiddlewareOptions
    {
        public List<IAnswer> Answers { get; set; }
    }
}

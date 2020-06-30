using System;
using System.Collections.Generic;
using System.Net;

namespace Texnomic.SecureDNS.Automation.Models
{
    public class Adapter
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public dynamic NameServers { get; set; }

        public int Index { get; set; }

        public string Status { get; set; }

        public string Mac { get; set; }

        public string Speed { get; set; }
    }
}

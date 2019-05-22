using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Texnomic.DNS.Protocol;

namespace Texnomic.SecureDNS.Models
{
    public class Host
    {
        public int ID { get; set; }
        public Domain Domain { get; set; }
        public IPAddress IPAddress { get; set; }
    }
}

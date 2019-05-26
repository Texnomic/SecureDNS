using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Texnomic.SecureDNS.Models
{
    public class Resolver
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        public Hexadecimal Hash { get; set; }
    }
}

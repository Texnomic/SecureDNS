using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Texnomic.DNS.Protocol;

namespace Texnomic.SecureDNS.Models
{
    public class Blacklist
    {
        public int ID { get; set; }
        public Domain Domain { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

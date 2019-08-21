using System;
using Texnomic.DNS.Models;

namespace Texnomic.SecureDNS.Data.Models
{
    public class Blacklist
    {
        public int ID { get; set; }
        public Domain Domain { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

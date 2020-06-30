using System;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Data.Models
{
    public class Blacklist
    {
        public int ID { get; set; }
        public IDomain Domain { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

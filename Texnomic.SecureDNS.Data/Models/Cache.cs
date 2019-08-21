using System;
using Texnomic.DNS.Models;

namespace Texnomic.SecureDNS.Data.Models
{
    public class Cache
    {
        public int ID { get; set; }
        public Domain Domain { get; set; }
        public Message Response { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

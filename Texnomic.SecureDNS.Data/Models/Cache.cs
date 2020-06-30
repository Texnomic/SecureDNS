using System;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Data.Models
{
    public class Cache
    {
        public int ID { get; set; }
        public IDomain Domain { get; set; }
        public IMessage Response { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

using System;
using Texnomic.DNS.Protocol;

namespace Texnomic.SecureDNS.Models
{
    public class Cache
    {
        public int ID { get; set; }
        public Domain Domain { get; set; }
        public Response Response { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

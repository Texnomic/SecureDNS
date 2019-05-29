using System.Net;
using Texnomic.DNS.Protocol;

namespace Texnomic.SecureDNS.Models
{
    public class Resolver
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Domain Domain { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        public Hexadecimal Hash { get; set; }
    }
}

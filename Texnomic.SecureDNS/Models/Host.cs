using System.Net;
using Texnomic.DNS.Models;

namespace Texnomic.SecureDNS.Models
{
    public class Host
    {
        public int ID { get; set; }
        public Domain Domain { get; set; }
        public IPAddress IPAddress { get; set; }
    }
}

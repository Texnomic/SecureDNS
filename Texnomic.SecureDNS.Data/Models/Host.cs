using System.Net;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Data.Models
{
    public class Host
    {
        public int ID { get; set; }
        public IDomain Domain { get; set; }
        public IPAddress IPAddress { get; set; }
    }
}

using System;

namespace Texnomic.DNS.Roots.Models
{
    public class Instance
    {
        public string Country { get; set; }
        public bool IPv4 { get; set; }
        public bool IPv6 { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Sites { get; set; }
        public string State { get; set; }
        public string Town { get; set; }
        public string Type { get; set; }
        public Uri[] Identifiers { get; set; }
    }
}

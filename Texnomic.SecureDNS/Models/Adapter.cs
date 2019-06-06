using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Texnomic.SecureDNS.Models
{
    public class Adapter
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Mac { get; set; }
        public string DNS { get; set; }
    }
}

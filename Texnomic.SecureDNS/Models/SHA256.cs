using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Texnomic.SecureDNS.Models
{
    public class SHA256
    {
        public byte[] Raw { get; private set; }

        public SHA256(byte[] Raw)
        {
            this.Raw = Raw;
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(Raw);
        }

        public override string ToString()
        {
            return ToBase64String();
        }
    }
}

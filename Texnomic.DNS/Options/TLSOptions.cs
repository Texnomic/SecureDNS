using System;
using System.Collections.Generic;
using System.Text;

namespace Texnomic.DNS.Options
{
    public class TLSOptions
    {
        public int Timeout { get; set; } = 2000;

        public string Host { get; set; } = "dns.google";

        public int Port { get; set; } = 853;

        public string PublicKey { get; set; }
    }
}

using System;
using System.Net;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Protocols.Options
{
    public class HTTPsOptions : IOptions
    {
        public int Retries { get; set; }

        public bool AllowRedirects { get; set; }

        public Uri Uri { get; set; }

        public WebProxy WebProxy { get; set; }

        public string PublicKey { get; set; }
    }
}

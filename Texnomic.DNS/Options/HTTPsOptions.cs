using System;
using System.Net;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Options
{
    public class HTTPsOptions : IOptions
    {
        public int Retries { get; set; } = 3;

        public bool AllowRedirects { get; set; } = false;

        public Uri Uri { get; set; } = new Uri($"https://dns.google:443/");

        public WebProxy WebProxy { get; set; }

        public string PublicKey { get; set; }
    }
}

using System;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Options
{
    public class ENSOptions : IOptions
    {
        public Uri Web3 { get; set; } = new Uri("https://cloudflare-eth.com/");
    }
}

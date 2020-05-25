using System;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.Options
{
    public class ENSOptions : IOptions
    {
        public Uri Web3 { get; set; } = new Uri("https://cloudflare-eth.com/");
    }
}

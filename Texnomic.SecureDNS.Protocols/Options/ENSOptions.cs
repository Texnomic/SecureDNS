using System;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Protocols.Options
{
    public class ENSOptions : IOptions
    {
        public Uri Web3 { get; set; }
    }
}

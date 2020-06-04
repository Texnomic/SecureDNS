using System;
using System.Collections.Generic;
using System.Text;

namespace Texnomic.DNS.Servers.Options
{
    public class ResolverMiddlewareOptions
    {
        public bool CacheEnabled { get; set; } = true;
        public double CacheCompactPercentage { get; set; } = 50.0;
        public double CacheCompactTimeout { get; set; } = 24 * 60 * 60 * 1000;
        public double CacheStatusTimeout { get; set; } = 60 * 60 * 1000;

    }
}

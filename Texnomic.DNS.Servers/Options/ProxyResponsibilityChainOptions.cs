using System;
using System.Collections.Generic;
using Texnomic.DNS.Servers.Middlewares;

namespace Texnomic.DNS.Servers.Options
{
    public class ProxyResponsibilityChainOptions
    {
        public List<Type> Middlewares { get; set; } = new List<Type>() { typeof(FilterMiddleware), typeof(GoogleHTTPsMiddleware) };
    }
}

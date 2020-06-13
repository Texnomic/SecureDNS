using System;
using System.Collections.Generic;
using System.Linq;
using PipelineNet.Middleware;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Servers.Proxy.Options
{
    public class ProxyResponsibilityChainOptions
    {
        public List<string> Middlewares { get; set; }

        public List<Type> GetMiddlewares() => AppDomain.CurrentDomain.GetAssemblies()
                                                                     .Single(Assembly => Assembly.FullName.StartsWith("Texnomic.SecureDNS.Middlewares")).GetTypes()
                                                                     .Where(Type => Type.GetInterfaces().Contains(typeof(IAsyncMiddleware<IMessage, IMessage>))).ToList();
    }
}

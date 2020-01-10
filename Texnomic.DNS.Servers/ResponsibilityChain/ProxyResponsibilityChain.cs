using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Servers.ResponsibilityChain
{
    public class ProxyResponsibilityChain : AsyncResponsibilityChain<IMessage, IMessage>
    {
        public ProxyResponsibilityChain(List<Type> Middlewares, IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {
            Middlewares.ForEach(Middleware => Chain(Middleware));
            Finally(Task.FromResult);
        }
    }
}

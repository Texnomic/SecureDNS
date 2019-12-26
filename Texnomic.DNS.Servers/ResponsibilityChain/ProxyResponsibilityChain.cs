using System;
using System.Threading.Tasks;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Servers.Middlewares;

namespace Texnomic.DNS.Servers.ResponsibilityChain
{
    public class ProxyResponsibilityChain : AsyncResponsibilityChain<IMessage, IMessage>
    {
        public ProxyResponsibilityChain(IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {

            Chain<Quad9TLSMiddleware>();

            //Chain<GoogleUDPMiddleware>();
            //Chain<ServerMiddleware>();

            Finally(Task.FromResult);
        }
    }
}

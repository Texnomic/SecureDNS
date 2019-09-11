using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Models;
using Texnomic.DNS.Servers.Middlewares;

namespace Texnomic.DNS.Servers.ResponsibilityChain
{
    public class ProxyResponsibilityChain : AsyncResponsibilityChain<Message, Message>

    {
        public ProxyResponsibilityChain(IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {
            Chain<GoogleUDPMiddleware>();
        }
    }
}

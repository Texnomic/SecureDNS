using System.Net;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Middlewares;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.ResponsibilityChain
{
    public class ProxyResponsibilityChain : AsyncResponsibilityChain<Message, Message>

    {
        public ProxyResponsibilityChain(IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {
            Chain<Quad9TLSMiddleware>();
        }
    }
}

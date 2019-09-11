using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Servers.Middlewares;

namespace Texnomic.DNS.Servers.ResponsibilityChain
{
    public class ServerResponsibilityChain : AsyncResponsibilityChain<Message, Message>

    {
        public ServerResponsibilityChain(IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {
            Chain<GoogleUDPMiddleware>();
            Chain<ServerMiddleware>();
        }
    }
}

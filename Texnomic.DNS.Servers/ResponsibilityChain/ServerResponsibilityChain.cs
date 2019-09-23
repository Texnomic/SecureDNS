using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Servers.Middlewares;

namespace Texnomic.DNS.Servers.ResponsibilityChain
{
    public class ServerResponsibilityChain : AsyncResponsibilityChain<IMessage, IMessage>

    {
        public ServerResponsibilityChain(IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {
            //Chain<GoogleUDPMiddleware>();
            Chain<Quad9TLSMiddleware>();
            //Chain<ServerMiddleware>();
        }
    }
}

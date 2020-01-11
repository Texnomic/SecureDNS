using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Servers.Options;

namespace Texnomic.DNS.Servers.ResponsibilityChain
{
    public class ProxyResponsibilityChain : AsyncResponsibilityChain<IMessage, IMessage>
    {
        public ProxyResponsibilityChain(IOptionsMonitor<ProxyResponsibilityChainOptions> Options, IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {
            Options.CurrentValue.Middlewares.ForEach(Middleware => Chain(Middleware));
            Finally(Task.FromResult);
        }
    }
}

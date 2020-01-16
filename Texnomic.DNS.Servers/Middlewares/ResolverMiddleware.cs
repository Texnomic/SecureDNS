using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class ResolverMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly IProtocol Protocol;

        public ResolverMiddleware(IProtocol Protocol)
        {
            this.Protocol = Protocol;
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            Message = await Protocol.ResolveAsync(Message);

            return await Next(Message);
        }
    }
}

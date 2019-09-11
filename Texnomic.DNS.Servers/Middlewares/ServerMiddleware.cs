using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class ServerMiddleware : IAsyncMiddleware<Message, Message>
    {
        private readonly IProtocol Protocol;

        public ServerMiddleware(IProtocol Protocol)
        {
            this.Protocol = Protocol;
        }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            Message = await Protocol.ResolveAsync(Message);

            return Next is null ? Message : await Next(Message);
        }
    }
}

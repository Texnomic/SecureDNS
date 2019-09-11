using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class ServerMiddleware<TProtocol> : IAsyncMiddleware<Message, Message> where TProtocol : IProtocol, new()
    {
        private readonly TProtocol Protocol;

        public ServerMiddleware(TProtocol Protocol)
        {
            this.Protocol = Protocol;
        }

        public Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class CircuitBreakerMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            return Message;
        }
    }
}

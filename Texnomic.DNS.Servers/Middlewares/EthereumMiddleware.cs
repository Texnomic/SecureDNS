using PipelineNet.Middleware;
using System;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class EthereumMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            throw new NotImplementedException();
        }
    }
}

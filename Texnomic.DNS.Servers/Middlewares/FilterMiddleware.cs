using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class FilterMiddleware : Filter, IAsyncMiddleware<IMessage, IMessage>
    {
        public FilterMiddleware() : base() { }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            Message = await ResolveAsync(Message);

            return Next is null ? Message : await Next(Message);
        }
    }
}

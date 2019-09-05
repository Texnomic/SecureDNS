using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Middlewares
{
    public class FilterMiddleware : Filter, IAsyncMiddleware<IMessage, IMessage>
    {
        public FilterMiddleware() : base() { }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            //Blocking Middleware
            return await ResolveAsync(Message);
        }
    }
}

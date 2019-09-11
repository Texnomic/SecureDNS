using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Middlewares
{
    public class FilterMiddleware : Filter, IAsyncMiddleware<Message, Message>
    {
        public FilterMiddleware() : base() { }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            //Blocking Middleware
            return await ResolveAsync(Message);
        }
    }
}

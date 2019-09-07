using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

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

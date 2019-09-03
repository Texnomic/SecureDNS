using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Middlewares
{
    public class UDPMiddleware : UDP, IAsyncMiddleware<Message, Message>
    {
        public UDPMiddleware(IPAddress IPAddress) : base(IPAddress) { }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            await ResolveAsync(Message);

            return await Next(Message);
        }
    }
}

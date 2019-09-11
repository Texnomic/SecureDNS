using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class UDPMiddleware : UDP, IAsyncMiddleware<Message, Message>
    {
        public UDPMiddleware(IPAddress IPAddress) : base(IPAddress) { }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            return await ResolveAsync(Message);
        }
    }
}

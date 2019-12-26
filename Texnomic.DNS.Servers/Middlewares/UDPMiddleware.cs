using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class UDPMiddleware : UDP, IAsyncMiddleware<IMessage, IMessage>
    {
        public UDPMiddleware(IPAddress IPAddress) : base(IPAddress) { }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            Message = await ResolveAsync(Message);

            return await Next(Message);
        }
    }
}

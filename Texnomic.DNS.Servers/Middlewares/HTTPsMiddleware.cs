using System;
using System.Net;
using System.Threading.Tasks;
using BinarySerialization;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class HTTPsMiddleware : HTTPs, IAsyncMiddleware<IMessage, IMessage>
    {
        public HTTPsMiddleware(IPAddress IPAddress, string PublicKey) : base(IPAddress, PublicKey)
        {
        }

        public HTTPsMiddleware(Uri Address, string PublicKey) : base(Address, PublicKey)
        {
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            Message = await ResolveAsync(Message);

            return await Next(Message);
        }
    }
}

using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Middlewares
{
    public class TLSMiddleware : TLS, IAsyncMiddleware<Message, Message>
    {
        public TLSMiddleware(IPAddress IPAddress, string PublicKey) : base(IPAddress, PublicKey) { }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            await ResolveAsync(Message);

            return await Next(Message);
        }
    }
}

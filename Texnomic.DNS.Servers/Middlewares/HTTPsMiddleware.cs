using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Middlewares
{
    public class HTTPsMiddleware : HTTPs, IAsyncMiddleware<Message, Message>
    {
        public HTTPsMiddleware(IPAddress IPAddress, string PublicKey) : base(IPAddress, PublicKey) { }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            var Bytes = await ResolveAsync(Message.ToArray());

            return await Next(Message.FromArray(Bytes));
        }
    }
}

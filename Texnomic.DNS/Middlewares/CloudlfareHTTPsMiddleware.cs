using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Middlewares
{
    public class CloudlfareHTTPsMiddleware : HTTPs, IAsyncMiddleware<Message, Message>
    {
        public CloudlfareHTTPsMiddleware() : base(IPAddress.Parse("1.1.1.1"), "") { }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            var Bytes = await ResolveAsync(Message.ToArray());

            return Message.FromArray(Bytes);
        }
    }
}

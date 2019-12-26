using System;
using System.Net;
using System.Threading.Tasks;
using BinarySerialization;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class CloudflareHTTPsMiddleware : HTTPs, IAsyncMiddleware<Message, Message>
    {
        private readonly BinarySerializer BinarySerializer;

        public CloudflareHTTPsMiddleware() : base(IPAddress.Parse("1.1.1.1"), "")
        {
            BinarySerializer = new BinarySerializer();
        }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            //Using Binary Format Over HTTPs 

            var RequestBytes = await BinarySerializer.SerializeAsync(Message);

            var ResponseBytes = await ResolveAsync(RequestBytes);

            Message = await BinarySerializer.DeserializeAsync<Message>(ResponseBytes);

            return await Next(Message);
        }
    }
}

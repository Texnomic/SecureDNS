using System;
using System.Net;
using System.Threading.Tasks;
using BinarySerialization;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class HTTPsMiddleware : HTTPs, IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly BinarySerializer BinarySerializer;

        public HTTPsMiddleware(IPAddress IPAddress, string PublicKey) : base(IPAddress, PublicKey)
        {
            BinarySerializer = new BinarySerializer();
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            //Using Binary Format Over HTTPs 

            var RequestBytes = await BinarySerializer.SerializeAsync(Message);

            var ResponseBytes = await ResolveAsync(RequestBytes);

            Message = await BinarySerializer.DeserializeAsync<Message>(ResponseBytes);

            return await Next(Message);
        }
    }
}

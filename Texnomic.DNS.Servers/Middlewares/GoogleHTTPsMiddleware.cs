using System;
using System.Net;
using System.Threading.Tasks;
using BinarySerialization;
using PipelineNet.Middleware;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class GoogleHTTPsMiddleware : HTTPs, IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly BinarySerializer BinarySerializer;

        public GoogleHTTPsMiddleware() : base(IPAddress.Parse("8.8.8.8"),
            "04C520708C204250281E7D44417C3079291C635E1D449BC5F7713A2BDED2A2A4B16C3D6AC877B8CB8F2E5053FDF418267F6137EDFFC2BEE90B5DB97EE1DF1CE274")
        {
            BinarySerializer = new BinarySerializer();
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            //Using Binary Format Over HTTPs 

            var RequestBytes = await BinarySerializer.SerializeAsync(Message);

            var ResponseBytes = await ResolveAsync(RequestBytes);

            Message = await BinarySerializer.DeserializeAsync<Message>(ResponseBytes);

            return Next is null ? Message : await Next(Message);
        }
    }
}

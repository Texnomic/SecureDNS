using System;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Texnomic.DNS.Models;
using Texnomic.DNS.Protocols;

namespace Texnomic.DNS.Middlewares
{
    public class GoogleHTTPsMiddleware : HTTPs, IAsyncMiddleware<Message, Message>
    {
        public GoogleHTTPsMiddleware() : base(IPAddress.Parse("8.8.8.8"),
            "04C520708C204250281E7D44417C3079291C635E1D449BC5F7713A2BDED2A2A4B16C3D6AC877B8CB8F2E5053FDF418267F6137EDFFC2BEE90B5DB97EE1DF1CE274") { }

        public async Task<Message> Run(Message Message, Func<Message, Task<Message>> Next)
        {
            var Bytes = await ResolveAsync(Message.ToArray());

            return Message.FromArray(Bytes);
        }
    }
}

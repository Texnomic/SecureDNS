using PipelineNet.Middleware;
using System;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Options;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class ENSMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly ILogger Logger;
        private readonly Protocols.ENS ENS;

        public ENSMiddleware(IOptionsMonitor<ENSOptions> Options, ILogger Logger, ILog Log) : base()
        {
            this.Logger = Logger;

            ENS = new Protocols.ENS(Options, Log);
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            if (!Message.Questions[0].Domain.Name.EndsWith(".eth")) 
                return await Next(Message);

            var Response = await ENS.ResolveAsync(Message);

            Logger.Information("Resolved ENS Query {@ID} For {@Domain}.", Message.ID, Message.Questions[0].Domain.Name);

            return Response;
        }
    }
}

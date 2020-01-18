using Microsoft.Extensions.Options;
using PipelineNet.Middleware;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Servers.Options;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class MasterFileMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly ILogger Logger;
        private readonly Lookup<string, IAnswer> Zone;

        public MasterFileMiddleware(IOptionsMonitor<MasterFileMiddlewareOptions> Options, ILogger Logger)
        {
            this.Logger = Logger;
            Zone = Options.CurrentValue.Answers.ToLookup(Answer => Answer.Domain.Name, Answer => Answer) as Lookup<string, IAnswer>;
        }


        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            throw new NotImplementedException();
        }
    }
}

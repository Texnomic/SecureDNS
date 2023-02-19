using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PipelineNet.Middleware;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Protocols.Options;

namespace Texnomic.SecureDNS.Middlewares;

public class ENSMiddleware : IAsyncMiddleware<IMessage, IMessage>
{
    private readonly ILogger Logger;
    private readonly SecureDNS.Protocols.ENS ENS;

    public ENSMiddleware(IOptionsMonitor<ENSOptions> Options, ILogger Logger) : base()
    {
        this.Logger = Logger;

        ENS = new Protocols.ENS(Options, Logger);
    }

    public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
    {
        if (!Message.Questions.First().Domain.Name.EndsWith(".eth")) 
            return await Next(Message);

        var Response = await ENS.ResolveAsync(Message);

        Logger.LogInformation("Resolved ENS Query {@ID} For {@Domain}.", Message.ID, Message.Questions.First().Domain.Name);

        return Response;
    }
}
namespace Texnomic.SecureDNS.Middlewares;

public class ENSMiddleware(IOptionsMonitor<ENSOptions> Options, ILogger Logger) : IAsyncMiddleware<IMessage, IMessage>
{
    private readonly Protocols.ENS ENS = new(Options);

    public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
    {
        if (!Message.Questions.First().Domain.Name.EndsWith(".eth")) 
            return await Next(Message);

        var Response = await ENS.ResolveAsync(Message);

        Logger.Information("Resolved ENS Query {ID} For {Domain}.", Message.ID, Message.Questions.First().Domain.Name);

        return Response;
    }
}
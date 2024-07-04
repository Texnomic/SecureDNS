namespace Texnomic.SecureDNS.Middlewares;

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
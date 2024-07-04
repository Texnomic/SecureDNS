namespace Texnomic.SecureDNS.Middlewares;

public class ResolverMiddleware : IAsyncMiddleware<IMessage, IMessage>, IDisposable
{
    private readonly IOptionsMonitor<ResolverMiddlewareOptions> Options;
    private readonly IProtocol Protocol;
    private readonly ILogger Logger;
    private readonly MemoryCache MemoryCache;
    private Timer CompactTimer;
    private Timer StatusTimer;

    public ResolverMiddleware(IOptionsMonitor<ResolverMiddlewareOptions> ResolverMiddlewareOptions, IProtocol Protocol, MemoryCache MemoryCache, ILogger Logger)
    {
        this.Protocol = Protocol;
        this.Logger = Logger;
        this.MemoryCache = MemoryCache;
        Options = ResolverMiddlewareOptions;
        Options.OnChange(Onchange);
    }

    private void Onchange(ResolverMiddlewareOptions ResolverMiddlewareOptions)
    {
        if (!ResolverMiddlewareOptions.CacheEnabled) return;

        CompactTimer = new Timer(ResolverMiddlewareOptions.CacheCompactTimeout * 60 * 60 * 1000);
        CompactTimer.Elapsed += CompactTimer_Elapsed;
        CompactTimer.Start();

        StatusTimer = new Timer(ResolverMiddlewareOptions.CacheStatusTimeout * 60 * 60 * 1000);
        StatusTimer.Elapsed += StatusTimer_Elapsed;
        StatusTimer.Start();
    }

    private void CompactTimer_Elapsed(object Sender, ElapsedEventArgs Args)
    {
        Logger.Information("Compacting {@Percentage} Of {@Count} From Memory Cache...", Options.CurrentValue.CacheCompactPercentage, MemoryCache.Count);

        MemoryCache.Compact(Options.CurrentValue.CacheCompactPercentage);

        Logger.Information("Compacted Memory Cache To {@Count}.", MemoryCache.Count);
    }

    private void StatusTimer_Elapsed(object Sender, ElapsedEventArgs Args)
    {
        Logger.Information("Resolver Memory Cache Contains {@Count} Messages.", MemoryCache.Count);
    }

    public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
    {
        if (!Options.CurrentValue.CacheEnabled)
            return await Next(Message);

        var Question = Message.Questions.First();

        var Cached = GetCache(Question.Domain, Question.Type);

        if (Cached == null)
        {
            Message = await Protocol.ResolveAsync(Message);

            SetCache(Message);

            return await Next(Message);
        }

        Cached.ID = Message.ID;

        Message = Cached;

        Logger.Information("Resolved Query {@ID} For {@Domain} From Cache.", Message.ID, Question.Domain.Name);

        return await Next(Message);
    }

    private void SetCache(IMessage Message)
    {
        if (Message.Truncated) return;

        if (Message.ResponseCode != ResponseCode.NoError) return;

        if (Message.AnswersCount == 0) return;

        var TimeToLive = Message.Answers.First().TimeToLive;

        if (TimeToLive != null)
            MemoryCache.Set($"{Message.Questions.First().Domain.Name}:{Message.Questions.First().Type}", Message, (TimeSpan)TimeToLive);
    }

    private IMessage GetCache(IDomain Domain, RecordType Type)
    {
        return MemoryCache.Get<IMessage>($"{Domain.Name}:{Type}");
    }

    private bool IsDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool Disposing)
    {
        if (IsDisposed) return;

        if (Disposing)
        {
            MemoryCache.Dispose();
            CompactTimer.Dispose();
        }

        IsDisposed = true;
    }

    ~ResolverMiddleware()
    {
        Dispose(false);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using PipelineNet.Middleware;

using Serilog;

using Texnomic.DNS.Servers.Options;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;

namespace Texnomic.DNS.Servers.Middlewares
{
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
            if (Options.CurrentValue.CacheEnabled)
            {
                var Answers = GetCache(Message);

                if (Answers == null)
                {
                    Message = await Protocol.ResolveAsync(Message);

                    SetCache(Message);

                    return await Next(Message);
                }

                Message = CreateAnswer(Message, Answers);

                Logger.Information("Resolved Query {@ID} For {@Domain} From Cache.", Message.ID, Message.Questions.First().Domain.Name);

                return await Next(Message);
            }

            Message = await Protocol.ResolveAsync(Message);

            return await Next(Message);
        }

        private void SetCache(IMessage Message)
        {
            if (Message.ResponseCode != ResponseCode.NoError) return;

            if (Message.AnswersCount == 0) return;

            MemoryCache.Set($"{Message.Questions.First().Domain.Name}:{Message.Questions.First().Type}", Message.Answers, Message.Answers.First().TimeToLive);
        }

        private List<IAnswer> GetCache(IMessage Message)
        {
            return MemoryCache.Get<List<IAnswer>>($"{Message.Questions.First().Domain.Name}:{Message.Questions.First().Type}");
        }

        private static IMessage CreateAnswer(IMessage Query, IReadOnlyCollection<IAnswer> Answers)
        {
            return new Message()
            {
                ID = Query.ID,
                MessageType = MessageType.Response,
                OperationCode = OperationCode.Query,
                AuthoritativeAnswer = AuthoritativeAnswer.Cache,
                Truncated = false,
                RecursionDesired = Query.RecursionDesired,
                RecursionAvailable = Query.RecursionAvailable,
                ResponseCode = ResponseCode.NoError,
                QuestionsCount = Query.QuestionsCount,
                AnswersCount = (ushort)Answers.Count,
                Questions = Query.Questions,
                Answers = Answers
            };
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
}

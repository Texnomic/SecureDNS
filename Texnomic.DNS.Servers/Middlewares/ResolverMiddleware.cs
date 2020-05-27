using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Caching.Memory;
using PipelineNet.Middleware;
using Serilog;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class ResolverMiddleware : IAsyncMiddleware<IMessage, IMessage>, IDisposable
    {
        private readonly IProtocol Protocol;
        private readonly ILogger Logger;
        private readonly MemoryCache MemoryCache;
        private readonly Timer Timer;

        public ResolverMiddleware(IProtocol Protocol, MemoryCache MemoryCache, ILogger Logger)
        {
            this.Protocol = Protocol;
            this.Logger = Logger;
            this.MemoryCache = MemoryCache;
            Timer = new Timer(60 * 60 * 1000);
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        private void Timer_Elapsed(object Sender, ElapsedEventArgs Args)
        {
            Logger.Information("Compacting 50% Of {@Count} From Memory Cache...", MemoryCache.Count);

            MemoryCache.Compact(50.0);

            Logger.Information("Compacted Memory Cache To {@Count}.", MemoryCache.Count);
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            var Answers = GetCache(Message);

            if (Answers == null)
            {
                Message = await Protocol.ResolveAsync(Message);

                SetCache(Message);

                return await Next(Message);
            }
            else
            {
                Message = CreateAnswer(Message);

                Message.Answers = Answers;

                Logger.Information("Resolved Query {@ID} For {@Domain} From Cache.", Message.ID, Message.Questions.First().Domain.Name);

                return await Next(Message);
            }
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

        private static IMessage CreateAnswer(IMessage Query)
        {
            return new Message()
            {
                ID = Query.ID,
                MessageType = MessageType.Response,
                ResponseCode = ResponseCode.NoError,
                Questions = Query.Questions,
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
                Timer.Dispose();
            }

            IsDisposed = true;
        }

        ~ResolverMiddleware()
        {
            Dispose(false);
        }
    }
}

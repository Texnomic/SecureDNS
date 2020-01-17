using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BinarySerialization;
using Microsoft.Extensions.Caching.Memory;
using PipelineNet.Middleware;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Records;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class ResolverMiddleware : IAsyncMiddleware<IMessage, IMessage>, IDisposable
    {
        private readonly IProtocol Protocol;
        private readonly ILogger Logger;
        private readonly MemoryCache MemoryCache;
        private readonly BinarySerializer BinarySerializer;
        private readonly Timer Timer;

        public ResolverMiddleware(IProtocol Protocol, MemoryCache MemoryCache, ILogger Logger)
        {
            this.Protocol = Protocol;
            this.Logger = Logger;
            this.MemoryCache = MemoryCache;
            BinarySerializer = new BinarySerializer();
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

                Logger.Information("Resolved Query {@ID} For {@Domain} From Cache.", Message.ID, Message.Questions[0].Domain.Name);

                return await Next(Message);
            }
        }

        private void SetCache(IMessage Message)
        {
            if (Message.ResponseCode != ResponseCode.NoError) return;

            if (Message.AnswersCount == 0) return;

            MemoryCache.Set($"{Message.Questions[0].Domain.Name}:{Message.Questions[0].Type}", Message.Answers, Message.Answers[0].TimeToLive.Value);
        }

        private List<IAnswer> GetCache(IMessage Message)
        {
            return MemoryCache.Get<List<IAnswer>>($"{Message.Questions[0].Domain.Name}:{Message.Questions[0].Type}");
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

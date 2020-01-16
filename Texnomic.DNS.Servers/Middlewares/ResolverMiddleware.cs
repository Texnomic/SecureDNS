using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using BinarySerialization;
using Microsoft.Extensions.Caching.Memory;
using PipelineNet.Middleware;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

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
            var Answers = TryGetCache(Message);

            if (Answers.Count == 0)
            {
                Message = await Protocol.ResolveAsync(Message);

                Cache(Message.Answers);

                return await Next(Message);
            }
            else
            {
                if (Answers.Count == Message.QuestionsCount)
                {
                    Message = CreateAnswer(Message);

                    Message.Answers = Answers;

                    Logger.Information("Query {@ID} Resolved From Cache.", Message.ID);

                    return await Next(Message);
                }
                else
                {
                    return await Next(Message);
                }
            }
        }

        public void Cache(List<IAnswer> Answers)
        {
            foreach (var Answer in Answers)
            {
                MemoryCache.Set($"{Answer.Domain.Name}:{Answer.Type}", Answer, Answer.TimeToLive.Value);

                Logger.Verbose("Added {@Answers} To Cache.", Answers);
            }
        }

        public List<IAnswer> TryGetCache(IMessage Message)
        {
            var Answers = new List<IAnswer>();

            foreach (var Question in Message.Questions)
            {
                var Result = MemoryCache.TryGetValue<Answer>($"{Question.Domain.Name}:{Question.Type}", out var Answer);

                if (Result) Answers.Add(Answer);
            }

            return Answers;
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

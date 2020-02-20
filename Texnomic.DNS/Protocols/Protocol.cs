using System;
using System.Threading.Tasks;
using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Protocols
{
    public abstract class Protocol : IProtocol
    {
        protected readonly BinarySerializer BinarySerializer;

        protected Protocol()
        {
            BinarySerializer = new BinarySerializer();
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public IMessage Resolve(IMessage Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public abstract Task<byte[]> ResolveAsync(byte[] Query);

        public async Task<IMessage> ResolveAsync(IMessage Query)
        {
            var SerializedQuery = await BinarySerializer.SerializeAsync(Query);

            var SerializedAnswer = await ResolveAsync(SerializedQuery);

            var AnswerMessage = await BinarySerializer.DeserializeAsync<Message>(SerializedAnswer);

            return AnswerMessage;
        }

        protected bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool Disposing);

        ~Protocol()
        {
            Dispose(false);
        }
    }
}

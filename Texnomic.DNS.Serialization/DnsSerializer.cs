using System;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Serialization
{
    public static class DnsSerializer<TMessage> where TMessage : IMessage, new()
    {
        public static IMessage Serialize(ReadOnlySpan<byte> RawMessage)
        {
            var Message = new TMessage();


            return Message;
        }
    }
}

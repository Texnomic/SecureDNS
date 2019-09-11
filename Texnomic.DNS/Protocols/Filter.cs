using System;
using System.Threading.Tasks;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Protocols
{
    public class Filter : IProtocol
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Filter()
        {
            
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            throw new NotImplementedException();
        }

        public async Task<Message> ResolveAsync(Message Query)
        {
            throw new NotImplementedException();
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public Message Resolve(Message Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }
    }
}

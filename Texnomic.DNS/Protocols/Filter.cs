using System;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Extensions;

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

        public async Task<IMessage> ResolveAsync(IMessage Query)
        {
            throw new NotImplementedException();
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public IMessage Resolve(IMessage Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }
    }
}

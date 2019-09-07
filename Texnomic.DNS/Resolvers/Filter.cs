using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public class Filter : IResolver
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

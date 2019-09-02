using System;
using System.Threading.Tasks;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public interface IResolver : IDisposable
    {
        byte[] Resolve(byte[] Query);
        Message Resolve(Message Query);

        ValueTask<byte[]> ResolveAsync(byte[] Query);
        ValueTask<Message> ResolveAsync(Message Query);
    }
}

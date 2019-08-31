using System;
using System.Threading.Tasks;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public interface IResolver : IDisposable
    {
        Task<byte[]> ResolveAsync(byte[] Query);
        Task<Message> ResolveAsync(Message Query);
    }
}

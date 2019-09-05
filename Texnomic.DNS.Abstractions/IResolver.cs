using System;
using System.Threading.Tasks;

namespace Texnomic.DNS.Abstractions
{
    public interface IResolver : IDisposable
    {
        byte[] Resolve(byte[] Query);
        IMessage Resolve(IMessage Query);

        Task<byte[]> ResolveAsync(byte[] Query);
        Task<IMessage> ResolveAsync(IMessage Query);
    }
}

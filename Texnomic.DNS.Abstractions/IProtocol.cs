using System;
using System.Threading.Tasks;

namespace Texnomic.DNS.Abstractions
{
    public interface IProtocol : IDisposable
    {
        Task<byte[]> ResolveAsync(byte[] Query);

        Task<IMessage> ResolveAsync(IMessage Query);

        byte[] Resolve(byte[] Query);

        IMessage Resolve(IMessage Query);
    }
}

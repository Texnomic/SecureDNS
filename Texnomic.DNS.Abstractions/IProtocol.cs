using System;
using System.Threading.Tasks;

namespace Texnomic.DNS.Abstractions
{
    public interface IProtocol : IDisposable
    {
        ValueTask<byte[]> ResolveAsync(byte[] Query);

        ValueTask<IMessage> ResolveAsync(IMessage Query);

        byte[] Resolve(byte[] Query);

        IMessage Resolve(IMessage Query);
    }
}

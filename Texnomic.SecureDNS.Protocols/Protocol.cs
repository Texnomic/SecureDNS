using System;
using System.Threading;
using System.Threading.Tasks;

using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Serialization;

namespace Texnomic.SecureDNS.Protocols;

public abstract class Protocol : IProtocol
{
    protected bool IsInitialized;

    protected CancellationTokenSource CancellationTokenSource;

    protected virtual void OptionsOnChange(IOptions Options)
    {
        throw new NotImplementedException();
    }

    protected virtual ValueTask InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public byte[] Resolve(byte[] Query)
    {
        return Async.RunSync(() => ResolveAsync(Query).AsTask());
    }

    public IMessage Resolve(IMessage Query)
    {
        return Async.RunSync(() => ResolveAsync(Query).AsTask());
    }

    public abstract ValueTask<byte[]> ResolveAsync(byte[] Query);

    public virtual async ValueTask<IMessage> ResolveAsync(IMessage Query)
    {
        var SerializedQuery = DnSerializer.Serialize(Query);

        var SerializedAnswer = await ResolveAsync(SerializedQuery);

        var AnswerMessage = DnSerializer.Deserialize(SerializedAnswer);

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
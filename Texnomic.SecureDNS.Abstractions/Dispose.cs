using System;

namespace Texnomic.SecureDNS.Abstractions;

public class Disposable : IDisposable
{
    // Flag: Has Dispose already been called?
    private bool IsDisposed;

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool Disposing)
    {
        if (IsDisposed) return;

        if (Disposing)
        {
            // Free any other managed objects here.
        }

        // Free any unmanaged objects here.
        //
        IsDisposed = true;
    }

    ~Disposable()
    {
        Dispose(false);
    }
}
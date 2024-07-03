namespace Texnomic.SecureDNS.Protocols;

public class UDP(IOptionsMonitor<UDPOptions> Options) : Protocol
{
    public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
    {
        using var Socket = new Socket(SocketType.Dgram, ProtocolType.Udp);

        Socket.ReceiveTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds;
        Socket.SendTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds;

        await Socket.ConnectAsync(Options.CurrentValue.IPv4EndPoint);

        var Buffer = new byte[1024];

        await Socket.SendAsync(Query, SocketFlags.None);
            
        var Size = await Socket.ReliableReceiveAsync(Buffer);
            
        return Buffer[..Size];
    }

    protected override void Dispose(bool Disposing)
    {
        if (IsDisposed) 
            return;

        IsDisposed = true;
    }
}
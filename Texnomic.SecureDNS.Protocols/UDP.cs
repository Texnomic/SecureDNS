using System.Net.Sockets;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Protocols.Options;


namespace Texnomic.SecureDNS.Protocols;

public class UDP : Protocol
{
    private readonly IOptionsMonitor<UDPOptions> Options;

    public UDP(IOptionsMonitor<UDPOptions> Options)
    {
        this.Options = Options;
    }

    public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
    {
        using var Socket = new Socket(SocketType.Dgram, ProtocolType.Udp)
        {
            ReceiveTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds,
            SendTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds
        };

        await Socket.ConnectAsync(Options.CurrentValue.IPv4EndPoint);

        var Buffer = new byte[1024];

        await Socket.SendAsync(Query, SocketFlags.None);
            
        var Size = await Socket.ReliableReceiveAsync(Buffer);
            
        return Buffer[..Size];
    }

    protected override void Dispose(bool Disposing)
    {
        if (IsDisposed) return;

        if (Disposing)
        {

        }

        IsDisposed = true;
    }
}
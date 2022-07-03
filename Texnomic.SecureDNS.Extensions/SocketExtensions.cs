using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Texnomic.SecureDNS.Extensions;

public static class SocketExtensions
{
    public static async Task<int> ReliableReceiveAsync(this Socket Socket, ArraySegment<byte> Buffer)
    {
        var Retries = 0;

        var Size = 0;

        while (Buffer[0] == 0 && Retries <= 5)
        {
            if (Retries > 0)
                await Task.Delay(100);

            Retries++;

            Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);
        }

        return Size;
    }

}
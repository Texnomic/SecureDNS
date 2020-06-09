using System;
using System.Net.Security;
using System.Threading.Tasks;

namespace Texnomic.SecureDNS.Extensions
{
    public static class SslStreamExtensions
    {
        public static async Task<int> ReliableReadAsync(this SslStream Stream, ArraySegment<byte> Buffer)
        {
            var Retries = 0;

            var Size = 0;

            while (Buffer[0] == 0 && Retries <= 5)
            {
                if (Retries > 0)
                    await Task.Delay(100);

                Retries++;

                Size = await Stream.ReadAsync(Buffer);
            }

            return Size;
        }
    }
}

using System.Buffers.Binary;
using System.Net.Sockets;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Protocols.Options;


namespace Texnomic.SecureDNS.Protocols
{
    public class TCP : Protocol
    {
        private readonly IOptionsMonitor<TCPOptions> Options;

        public TCP(IOptionsMonitor<TCPOptions> TCPOptions)
        {
            Options = TCPOptions;
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            using var Socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds,
                SendTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds
            };

            await Socket.ConnectAsync(Options.CurrentValue.IPv4EndPoint);

            var Prefix = new byte[2];

            BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort)Query.Length);

            await Socket.SendAsync(Prefix, SocketFlags.None);

            await Socket.SendAsync(Query, SocketFlags.None);

            await Socket.ReceiveAsync(Prefix, SocketFlags.None);

            var Size = BinaryPrimitives.ReadUInt16BigEndian(Prefix);

            var Buffer = new byte[Size];

            await Socket.ReliableReceiveAsync(Buffer);

            return Buffer;
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
}

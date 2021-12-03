using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
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

            //await Socket.ConnectAsync(Options.CurrentValue.IPv4EndPoint);

            //var Prefix = new byte[2];

            //BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort)Query.Length);

            //await Socket.SendAsync(Prefix, SocketFlags.None);

            //await Socket.SendAsync(Query, SocketFlags.None);

            //await Socket.ReceiveAsync(Prefix, SocketFlags.None);

            //var Size = BinaryPrimitives.ReadUInt16BigEndian(Prefix);

            //var Buffer = new byte[Size];

            //await Socket.ReliableReceiveAsync(Buffer);

            //return Buffer;

            await Socket.ConnectAsync(Options.CurrentValue.IPv4EndPoint);

            var Stream = new NetworkStream(Socket);

            var Reader = PipeReader.Create(Stream);

            var Writer = PipeWriter.Create(Stream);

            var CancellationTokenSource = new CancellationTokenSource(Options.CurrentValue.Timeout);

            var Prefix = new byte[2];

            BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort)Query.Length);

            await Writer.WriteAsync(Prefix, CancellationTokenSource.Token);

            await Writer.WriteAsync(Query, CancellationTokenSource.Token);

            await Writer.FlushAsync(CancellationTokenSource.Token);

            while (true)
            {
                var Result = await Reader.ReadAsync(CancellationTokenSource.Token);

                if (Result.IsCompleted)
                {
                    var Size = BinaryPrimitives.ReadUInt16BigEndian(Result.Buffer.FirstSpan.Slice(0, 2));

                    return Result.Buffer.FirstSpan.Slice(2, Size).ToArray();
                }

                await Task.Delay(100, CancellationTokenSource.Token);
            }
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

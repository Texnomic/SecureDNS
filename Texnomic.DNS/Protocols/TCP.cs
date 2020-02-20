using System;
using Nerdbank.Streams;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Texnomic.DNS.Options;
using Microsoft.Extensions.Options;

namespace Texnomic.DNS.Protocols
{
    public class TCP : Protocol
    {
        private readonly IOptionsMonitor<TCPOptions> Options;
        private readonly TcpClient TcpClient;

        private NetworkStream NetworkStream;
        private PipeReader PipeReader;
        private PipeWriter PipeWriter;

        public TCP(IOptionsMonitor<TCPOptions> TCPOptions)
        {
            Options = TCPOptions;

            TcpClient = new TcpClient();
        }

        protected override async ValueTask InitializeAsync()
        {
            await TcpClient.ConnectAsync(Options.CurrentValue.Host, Options.CurrentValue.Port);

            NetworkStream = TcpClient.GetStream();

            PipeReader = NetworkStream.UsePipeReader();

            PipeWriter = NetworkStream.UsePipeWriter();
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            if (!TcpClient.Connected || !NetworkStream.CanWrite) await InitializeAsync();

            var QueryLength = BitConverter.GetBytes((ushort)Query.Length);

            var PrefixedQuery = Concat(QueryLength, Query);

            await PipeWriter.WriteAsync(PrefixedQuery);

            PipeWriter.Complete();

            var Task = PipeReader.ReadAsync().AsTask();

            Task.Wait(Options.CurrentValue.Timeout);

            if (Task.IsCompleted)
            {
                var Result = Task.Result;

                PipeReader.Complete();

                var Buffer = Result.Buffer.Length > 14
                    ? Result.Buffer.Slice(2)
                    : throw new OperationCanceledException();

                return Buffer.ToArray();
            }

            PipeReader.CancelPendingRead();

            PipeReader.Complete();

            throw new TimeoutException();
        }


        private static T[] Concat<T>(params T[][] Arrays)
        {
            var Result = new T[Arrays.Sum(A => A.Length)];

            var Offset = 0;

            foreach (var Array in Arrays)
            {
                Array.CopyTo(Result, Offset);

                Offset += Array.Length;
            }

            return Result;
        }

        protected override void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                NetworkStream.Dispose();
                TcpClient.Dispose();
            }

            IsDisposed = true;
        }
    }
}

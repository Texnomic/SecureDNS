using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nerdbank.Streams;
using Texnomic.SecureDNS.Core.Options;

namespace Texnomic.SecureDNS.Protocols
{
    public class TLS : Protocol
    {
        private readonly IOptionsMonitor<TLSOptions> Options;
        private readonly TcpClient TcpClient;

        private SslStream SslStream;
        private PipeReader PipeReader;
        private PipeWriter PipeWriter;

        public TLS(IOptionsMonitor<TLSOptions> TLSOptions)
        {
            Options = TLSOptions;

            TcpClient = new TcpClient();
        }

        protected override async ValueTask InitializeAsync()
        {
            await TcpClient.ConnectAsync(Options.CurrentValue.Host, Options.CurrentValue.Port);

            SslStream = new SslStream(TcpClient.GetStream(), true, ValidateServerCertificate)
            {
                ReadTimeout = Options.CurrentValue.Timeout,
                WriteTimeout = Options.CurrentValue.Timeout
            };

            await SslStream.AuthenticateAsClientAsync(Options.CurrentValue.Host);

            PipeReader = SslStream.UsePipeReader();

            PipeWriter = SslStream.UsePipeWriter();
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            if (!TcpClient.Connected || !SslStream.CanWrite) await InitializeAsync();

            var QueryLength = BitConverter.GetBytes((ushort)Query.Length);

            var PrefixedQuery = Concat(QueryLength, Query);

            await PipeWriter.WriteAsync(PrefixedQuery);

            await PipeWriter.CompleteAsync();

            var Task = PipeReader.ReadAsync().AsTask();

            Task.Wait(Options.CurrentValue.Timeout);

            if (Task.IsCompleted)
            {
                var Result = Task.Result;

                await PipeReader.CompleteAsync();

                var Buffer = Result.Buffer.Length > 14
                    ? Result.Buffer.Slice(2)
                    : throw new OperationCanceledException();

                return Buffer.ToArray();
            }

            PipeReader.CancelPendingRead();

            await PipeReader.CompleteAsync();

            throw new TimeoutException();
        }


        private bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            return string.IsNullOrEmpty(Options.CurrentValue.PublicKey) ? SslPolicyErrors == SslPolicyErrors.None : SslPolicyErrors == SslPolicyErrors.None && Certificate.GetPublicKeyString() == Options.CurrentValue.PublicKey;
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
                SslStream.Dispose();
                TcpClient.Dispose();
            }

            IsDisposed = true;
        }
    }
}

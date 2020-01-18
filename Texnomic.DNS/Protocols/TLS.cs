using System;
using Nerdbank.Streams;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Options;
using Microsoft.Extensions.Options;

namespace Texnomic.DNS.Protocols
{
    public class TLS : IProtocol
    {
        private readonly BinarySerializer BinarySerializer;
        private readonly IOptionsMonitor<TLSOptions> Options;
        private readonly TcpClient TcpClient;
        private readonly SslStream SslStream;
        private readonly PipeReader PipeReader;
        private readonly PipeWriter PipeWriter;

        public TLS(IOptionsMonitor<TLSOptions> TLSOptions)
        {
            Options = TLSOptions;

            BinarySerializer = new BinarySerializer();

            TcpClient = new TcpClient();

            SslStream = new SslStream(TcpClient.GetStream(), true, ValidateServerCertificate)
            {
                ReadTimeout = Options.CurrentValue.Timeout,
                WriteTimeout = Options.CurrentValue.Timeout
            };

            PipeReader = SslStream.UsePipeReader();

            PipeWriter = SslStream.UsePipeWriter();
        }

        private async Task InitializeAsync()
        {
            await TcpClient.ConnectAsync(Options.CurrentValue.Host, Options.CurrentValue.Port);

            await SslStream.AuthenticateAsClientAsync(Options.CurrentValue.Host);
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public IMessage Resolve(IMessage Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public async Task<byte[]> ResolveAsync(byte[] Request)
        {
            if (!TcpClient.Connected || !SslStream.CanWrite) await InitializeAsync();

            var Length = BitConverter.GetBytes((ushort)Request.Length);

            Array.Reverse(Length);

            await PipeWriter.WriteAsync(Length);

            await PipeWriter.WriteAsync(Request);

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
            else
            {
                PipeReader.CancelPendingRead();

                PipeReader.Complete();

                throw new TimeoutException();
            }
        }

        public async Task<IMessage> ResolveAsync(IMessage Request)
        {
            if (!TcpClient.Connected || !SslStream.CanWrite) await InitializeAsync();

            var Bytes = await BinarySerializer.SerializeAsync(Request);

            var Length = BitConverter.GetBytes((ushort)Bytes.Length);

            Array.Reverse(Length);

            await PipeWriter.WriteAsync(Length);

            await PipeWriter.WriteAsync(Bytes);

            PipeWriter.Complete();

            var Task = PipeReader.ReadAsync().AsTask();

            Task.Wait(Options.CurrentValue.Timeout);

            if (Task.IsCompleted)
            {
                var Result = Task.Result;

                var Buffer = Result.Buffer.Length > 14
                    ? Result.Buffer.Slice(2)
                    : throw new OperationCanceledException();

                var Response = await BinarySerializer.DeserializeAsync<Message>(Buffer);

                PipeReader.Complete();

                return Response;
            }
            else
            {
                PipeReader.CancelPendingRead();

                PipeReader.Complete();

                throw new TimeoutException();
            }
        }


        private bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            return string.IsNullOrEmpty(Options.CurrentValue.PublicKey) ? SslPolicyErrors == SslPolicyErrors.None : SslPolicyErrors == SslPolicyErrors.None && Certificate.GetPublicKeyString() == Options.CurrentValue.PublicKey;
        }

        private bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                SslStream.Dispose();
                TcpClient.Dispose();
            }

            IsDisposed = true;
        }

        ~TLS()
        {
            Dispose(false);
        }
    }
}

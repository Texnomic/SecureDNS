using System;
using Nerdbank.Streams;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using Nethereum.ABI;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Protocols
{
    public class TLS : IProtocol
    {
        private readonly BinarySerializer BinarySerializer;
        private readonly string PublicKey;
        private readonly IPAddress IPAddress;

        private TcpClient TcpClient;
        private SslStream SslStream;
        private PipeReader PipeReader;
        private PipeWriter PipeWriter;

        private const int Timeout = 2000;

        public TLS(IPAddress IPAddress, string PublicKey)
        {
            BinarySerializer = new BinarySerializer();
            this.IPAddress = IPAddress;
            this.PublicKey = PublicKey;
            Async.RunSync(InitializeAsync);
        }

        private async Task InitializeAsync()
        {
            TcpClient = new TcpClient();

            await TcpClient.ConnectAsync(IPAddress, 853);

            SslStream = new SslStream(TcpClient.GetStream(), true, ValidateServerCertificate);

            await SslStream.AuthenticateAsClientAsync(IPAddress.ToString());

            SslStream.ReadTimeout = Timeout;

            SslStream.WriteTimeout = Timeout;

            PipeReader = SslStream.UsePipeReader();

            PipeWriter = SslStream.UsePipeWriter();
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

            Task.Wait(Timeout);

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

            Task.Wait(Timeout);

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
            return SslPolicyErrors == SslPolicyErrors.None && Certificate.GetPublicKeyString() == PublicKey;
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

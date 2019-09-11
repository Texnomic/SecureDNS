using System;
using Nerdbank.Streams;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BinarySerialization;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Protocols
{
    public class TLS : IProtocol
    {
        private readonly BinarySerializer Serializer;
        private readonly string PublicKey;
        private readonly IPAddress IPAddress;

        private TcpClient TcpClient;
        private SslStream SslStream;
        private PipeReader PipeReader;
        private PipeWriter PipeWriter;

        public TLS(IPAddress IPAddress, string PublicKey)
        {
            Serializer = new BinarySerializer();
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

            PipeReader = SslStream.UsePipeReader();

            PipeWriter = SslStream.UsePipeWriter();
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public Message Resolve(Message Query)
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

            var Result = await PipeReader.ReadAsync();

            PipeReader.Complete();

            return Result.Buffer.Slice(2).ToArray();
        }

        public async Task<Message> ResolveAsync(Message Request)
        {
            if (!TcpClient.Connected || !SslStream.CanWrite) await InitializeAsync();

            var Bytes = Request.ToArray();

            var Length = BitConverter.GetBytes((ushort)Bytes.Length);

            Array.Reverse(Length);

            await PipeWriter.WriteAsync(Length);

            await PipeWriter.WriteAsync(Bytes);

            PipeWriter.Complete();

            var Result = await PipeReader.ReadAsync();

            var Response = Message.FromArray(Result.Buffer.Slice(2));

            PipeReader.Complete();

            return Response;
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

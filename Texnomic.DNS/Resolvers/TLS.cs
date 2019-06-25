using Nerdbank.Streams;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public class TLS : IResolver, IDisposable
    {
        private readonly string PublicKey;
        private readonly IPAddress IPAddress;

        private TcpClient TcpClient;
        private SslStream SslStream;
        private PipeReader PipeReader;
        private PipeWriter PipeWriter;

        public TLS(IPAddress IPAddress, string PublicKey)
        {
            this.IPAddress = IPAddress;
            this.PublicKey = PublicKey;
            Task.WaitAll(InitializeAsync());
        }

        private async Task InitializeAsync()
        {
            try
            {
                TcpClient = new TcpClient();

                await TcpClient.ConnectAsync(IPAddress, 853);

                SslStream = new SslStream(TcpClient.GetStream(), true, ValidateServerCertificate);

                await SslStream.AuthenticateAsClientAsync(IPAddress.ToString());

                PipeReader = SslStream.UsePipeReader();

                PipeWriter = SslStream.UsePipeWriter();
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public async Task<byte[]> ResolveAsync(byte[] Request)
        {
            try
            {
                if (!TcpClient.Connected || !SslStream.CanWrite) await InitializeAsync();

                await PipeWriter.WriteAsync(Request);

                PipeWriter.Complete();

                var Result = await PipeReader.ReadAsync();

                PipeReader.Complete();

                return Result.Buffer.ToArray();
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public async Task<Message> ResolveAsync(Message Request)
        {
            try
            {
                if (!TcpClient.Connected || !SslStream.CanWrite) await InitializeAsync();

                var Bytes = Request.ToArray();

                await PipeWriter.WriteAsync(Bytes);

                PipeWriter.Complete();

                var Result = await PipeReader.ReadAsync();

                var Response = Message.FromArray(Result.Buffer);

                PipeReader.Complete();

                return Response;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public void Dispose()
        {
            SslStream.Dispose();
            TcpClient.Dispose();
        }

        private bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            return SslPolicyErrors == SslPolicyErrors.None && Certificate.GetPublicKeyString() == PublicKey;
        }

    }
}

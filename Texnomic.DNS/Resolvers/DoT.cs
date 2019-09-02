using Nerdbank.Streams;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BinarySerialization;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Resolvers
{
    public class DoT : IResolver
    {
        private readonly BinarySerializer Serializer;
        private readonly string PublicKey;
        private readonly IPAddress IPAddress;

        private TcpClient TcpClient;
        private SslStream SslStream;
        private PipeReader PipeReader;
        private PipeWriter PipeWriter;

        public DoT(IPAddress IPAddress, string PublicKey)
        {
            Serializer = new BinarySerializer();
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

        public byte[] Resolve(byte[] Query)
        {
            throw new NotImplementedException();
        }

        public Message Resolve(Message Query)
        {
            throw new NotImplementedException();
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

                var Bytes = await Serializer.SerializeAsync(Request);

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

using System.Buffers.Binary;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Texnomic.SecureDNS.Protocols.Options;


namespace Texnomic.SecureDNS.Protocols
{
    public class TLS : Protocol
    {
        private readonly IOptionsMonitor<TLSOptions> Options;

        private Socket Socket;

        private NetworkStream NetworkStream;

        private SslStream SslStream;

        private byte[] Prefix;

        private readonly SemaphoreSlim SemaphoreSlim;

        public TLS(IOptionsMonitor<TLSOptions> TLSOptions)
        {
            Options = TLSOptions;

            Options.OnChange(OptionsOnChange);

            SemaphoreSlim = new SemaphoreSlim(1);

            IsInitialized = false;
        }

        private void OptionsOnChange(TLSOptions TLSOptions)
        {
            _ = InitializeAsync();
        }

        protected override async ValueTask InitializeAsync()
        {
            await SemaphoreSlim.WaitAsync();

            CancellationTokenSource = new CancellationTokenSource(Options.CurrentValue.Timeout);

            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            Prefix = new byte[2];

            await Socket.ConnectAsync(Options.CurrentValue.IPv4EndPoint);

            NetworkStream = new NetworkStream(Socket);

            SslStream = new SslStream(NetworkStream, true, ValidateServerCertificate);

            await SslStream.AuthenticateAsClientAsync(Options.CurrentValue.CommonName);

            IsInitialized = true;

            SemaphoreSlim.Release();
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            if (!IsInitialized) await InitializeAsync();

            await SemaphoreSlim.WaitAsync();

            BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort)Query.Length);

            await SslStream.WriteAsync(Prefix, CancellationTokenSource.Token);

            await SslStream.WriteAsync(Query, CancellationTokenSource.Token);

            await SslStream.ReadAsync(Prefix, CancellationTokenSource.Token);

            var Size = BinaryPrimitives.ReadUInt16BigEndian(Prefix);

            var Buffer = new byte[Size];

            await SslStream.ReadAsync(Buffer, CancellationTokenSource.Token);

            SemaphoreSlim.Release();

            return Buffer;
        }


        private bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            var X509Certificate2 = new X509Certificate2(Certificate);

            return string.IsNullOrEmpty(Options.CurrentValue.Thumbprint) ? SslPolicyErrors == SslPolicyErrors.None : SslPolicyErrors == SslPolicyErrors.None && X509Certificate2.Thumbprint == Options.CurrentValue.Thumbprint;
        }

        protected override void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                SslStream.Dispose();
                NetworkStream.Dispose();
                Socket.Dispose();
            }

            IsDisposed = true;
        }
    }
}

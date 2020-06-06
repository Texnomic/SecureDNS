using System.Buffers.Binary;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Texnomic.SecureDNS.Protocols.Options;


namespace Texnomic.SecureDNS.Protocols
{
    public class TLS : Protocol
    {
        private readonly IOptionsMonitor<TLSOptions> Options; 

        public TLS(IOptionsMonitor<TLSOptions> TLSOptions)
        {
            Options = TLSOptions;
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            using var Socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = (int) Options.CurrentValue.Timeout.TotalMilliseconds,
                SendTimeout = (int) Options.CurrentValue.Timeout.TotalMilliseconds
            };

            await Socket.ConnectAsync(Options.CurrentValue.IPv4EndPoint);

            await using var NetworkStream = new NetworkStream(Socket);

            var SslStream = new SslStream(NetworkStream, true, ValidateServerCertificate)
            {
                ReadTimeout = (int) Options.CurrentValue.Timeout.TotalMilliseconds,
                WriteTimeout = (int) Options.CurrentValue.Timeout.TotalMilliseconds,
            };

            await SslStream.AuthenticateAsClientAsync(Options.CurrentValue.CommonName);

            var Prefix = new byte[2];

            BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort) Query.Length);

            await SslStream.WriteAsync(Prefix);

            await SslStream.WriteAsync(Query);

            await SslStream.ReadAsync(Prefix);

            var Size = BinaryPrimitives.ReadUInt16BigEndian(Prefix);

            var Buffer = new byte[Size];

            await SslStream.ReadAsync(Buffer);

            return Buffer;
        }


        private bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            var X509Certificate2 = new X509Certificate2(Certificate);

            return string.IsNullOrEmpty(Options.CurrentValue.Thumbprint) ? SslPolicyErrors == SslPolicyErrors.None : SslPolicyErrors == SslPolicyErrors.None &&X509Certificate2.Thumbprint == Options.CurrentValue.Thumbprint;
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

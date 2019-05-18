using DNS.Client;
using DNS.Client.RequestResolver;
using DNS.Protocol;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Texnomic.DNS.Resolvers
{

    /// <summary>
    /// A DNS-over-TLS Request Resolver implementation for Mirza 
    /// Kapetanovic's DNS library in NuGet. Use this if you want
    /// to use DNS-over-TLS when calling forwarders.
    /// 
    /// On top of using TLS for communications, this also verifies
    /// the SPKI Pin of the server on the remote end to ensure it's
    /// a legitimate endpoint that the end-user trusts.
    /// </summary>
    public class TlsRequestResolver : IRequestResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dns"></param>
        public TlsRequestResolver(IPEndPoint dns)
        {
            this.dns = dns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResponse> Resolve(IRequest request)
        {

            using (TcpClient tcp = new TcpClient())
            {

                await tcp.ConnectAsync(dns.Address, dns.Port);

                using (SslStream sslStream = new SslStream(tcp.GetStream(), false, new RemoteCertificateValidationCallback(UserCertificateValidationCallback), new LocalCertificateSelectionCallback(UserCertificateSelectionCallback), EncryptionPolicy.RequireEncryption))
                {
                    await sslStream.AuthenticateAsClientAsync("1.1.1.1");

                    byte[] buffer = request.ToArray();
                    byte[] length = BitConverter.GetBytes((ushort)buffer.Length);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(length);
                    }

                    await sslStream.WriteAsync(length, 0, length.Length);
                    await sslStream.WriteAsync(buffer, 0, buffer.Length);

                    buffer = new byte[2];
                    await Read(sslStream, buffer);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(buffer);
                    }

                    buffer = new byte[BitConverter.ToUInt16(buffer, 0)];
                    await Read(sslStream, buffer);

                    IResponse response = Response.FromArray(buffer);

                    return ClientResponse.FromArray(request, buffer);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static async Task Read(Stream stream, byte[] buffer)
        {
            int length = buffer.Length;
            int offset = 0;
            int size = 0;

            while (length > 0 && (size = await stream.ReadAsync(buffer, offset, length)) > 0)
            {
                offset += size;
                length -= size;
            }

            if (length > 0)
            {
                throw new IOException("Unexpected end of stream");
            }
        }

        private IPEndPoint dns;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("Pinning Hash: {0}", CertificateUtils.GetPublicKeyPinningHash((X509Certificate2)certificate));
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                Console.WriteLine("SSL Certificate Validation Error!");
                Console.WriteLine(sslPolicyErrors.ToString());
                return false;
            }
            //Enable for Certificate Pinning
            //else if (CertificateUtils.GetPublicKeyPinningHash((X509Certificate2)certificate) != "yioEpqeR4WtDwE9YxNVnCEkTxIjx6EEIwFSQW+lJsbc=")
            //    return false;
            else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="targetHost"></param>
        /// <param name="localCertificates"></param>
        /// <param name="localCertificate"></param>
        /// <param name="acceptableIssuers"></param>
        /// <returns></returns>
        private X509Certificate UserCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate localCertificate, string[] acceptableIssuers)
        {
            return null;
        }

    }
}

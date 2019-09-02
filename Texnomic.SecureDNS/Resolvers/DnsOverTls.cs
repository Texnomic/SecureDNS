using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Texnomic.DNS.Resolvers;
using Texnomic.DNS.Models;

namespace Texnomic.SecureDNS.Resolvers
{
    public class DnsOverTls : IResolver
    {
        private readonly DatabaseContext DatabaseContext;

        private TcpClient TcpClient;
        private SslStream SslStream;

        public List<Host> Hosts;
        public List<Cache> Cache;
        public List<Resolver> Resolvers;

        //Generic Workaround, Needs Fixing
        public DnsOverTls()
        {
        }

        public DnsOverTls(DatabaseContext DatabaseContext)
        {
            this.DatabaseContext = DatabaseContext;
            InitializeAsync().RunSynchronously();
            PreloadAsync().RunSynchronously();
        }

        private async Task InitializeAsync()
        {
            foreach (var Resolver in Resolvers)
            {
                try
                {
                    TcpClient = new TcpClient();

                    await TcpClient.ConnectAsync(Resolver.IPAddress, 853);

                    SslStream = new SslStream(TcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));

                    await SslStream.AuthenticateAsClientAsync(Resolver.IPAddress.ToString());
                }
                catch (Exception Error)
                {
                    throw Error;
                }
            }
        }

        private async Task PreloadAsync()
        {
            Resolvers = await DatabaseContext.Resolvers.ToListAsync();
            Hosts = await DatabaseContext.Hosts.ToListAsync();
            Cache = await DatabaseContext.Cache.ToListAsync();
        }

        private async Task WriteAsync(Message Request)
        {
            while (true)
            {
                try
                {
                    var Buffer = Request.ToArray();

                    var Length = BitConverter.GetBytes((ushort)Buffer.Length);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(Length);
                    }

                    await SslStream.WriteAsync(Length);
                    await SslStream.WriteAsync(Buffer);
                    await SslStream.FlushAsync();

                    break;
                }
                catch (Exception Error)
                {
                    throw Error;
                }
            }
        }

        private async Task<Message> ReadAsync()
        {
            while (true)
            {
                try
                {
                    var Buffer = new byte[2];

                    var Read = await SslStream.ReadAsync(Buffer);

                    if (Read == 0) throw new Exception("Read Zero Bytes from SslStream.");

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(Buffer);
                    }

                    var Length = BitConverter.ToUInt16(Buffer);

                    Buffer = new byte[Length];

                    Read = await SslStream.ReadAsync(Buffer);

                    if (Read == 0) throw new Exception("Read Zero Bytes from SslStream.");

                    return Message.FromArray(Buffer);
                }
                catch (Exception Error)
                {
                    throw Error;
                }
            }
        }

        public async Task<Message> ResolveAsync(Message Request)
        {
            try
            {
                var Domain = Request.Questions.First().Domain;

                var Host = Hosts.SingleOrDefault(Host => Host.Domain == Domain);

                if (Host != null) throw new NotImplementedException();

                var Cached = Cache.SingleOrDefault(Cache => Cache.Domain == Domain);

                if (Cached != null) return Cached.Response;

                var IsBlacklisted = await DatabaseContext.Blacklists.AnyAsync(Record => Record.Domain == Domain);

                if (IsBlacklisted) throw new NotImplementedException();

                if (!TcpClient.Connected || !SslStream.CanRead || !SslStream.CanWrite) await InitializeAsync();

                await WriteAsync(Request);

                var Response = await ReadAsync();

                await DatabaseContext.Cache
                                     .Upsert(new Cache() { Domain = Domain, Response = Response })
                                     .On(Record => Record.Domain)
                                     .RunAsync();

                return Response;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            return SslPolicyErrors == SslPolicyErrors.None && Resolvers.Any(Resolver => Resolver.Hash.ToString() == Certificate.GetPublicKeyString());
        }

        public void Dispose()
        {
            DatabaseContext.Dispose();
            SslStream.Dispose();
            TcpClient.Dispose();
        }

        public Task<byte[]> ResolveAsync(byte[] Request)
        {
            throw new NotImplementedException();
        }

        public byte[] Resolve(byte[] Query)
        {
            throw new NotImplementedException();
        }

        public Message Resolve(Message Query)
        {
            throw new NotImplementedException();
        }
    }
}

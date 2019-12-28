using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BinarySerialization;
using Polly;
using Polly.Retry;
using RestSharp;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Protocols
{
    /// <summary>
    /// DNS Over HTTPS <see href="https://tools.ietf.org/html/rfc8484">(DoH)</see>
    /// </summary>
    public class HTTPs : IProtocol
    {
        private readonly string PublicKey;

        private RestClient RestClient;
        private BinarySerializer BinarySerializer;
        private AsyncRetryPolicy<IRestResponse> RetryPolicy;
        private Random Random;

        public HTTPs(IPAddress IPAddress, string PublicKey)
        {
            this.PublicKey = PublicKey;

            Initialize(IPAddress.ToString());
        }

        public HTTPs(IPAddress IPAddress, int Port, string PublicKey)
        {
            this.PublicKey = PublicKey;

            Initialize(IPAddress.ToString(), Port);
        }

        public HTTPs(Uri Address, string PublicKey)
        {
            this.PublicKey = PublicKey;

            Initialize(Address.DnsSafeHost);
        }

        public HTTPs(Uri Address, int Port, string PublicKey)
        {
            this.PublicKey = PublicKey;

            Initialize(Address.DnsSafeHost, Port);
        }

        private void Initialize(string Address, int Port = 443)
        {
            Random = new Random();

            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;

            BinarySerializer = new BinarySerializer();

            RestClient = new RestClient($"https://{Address}:{Port}/");
            
            RestClient.AddDefaultHeader("Cache-Control", "no-cache");
            RestClient.AddDefaultHeader("Accept", "application/dns-message");

            //RestClient.Proxy = new WebProxy("127.0.0.1:8888"); //Debugging with Fiddler
            //RestClient.FollowRedirects = false; //Google will fail

            RetryPolicy = Policy.HandleResult<IRestResponse>(ResultPredicate)
                                .RetryAsync(3);
        }

        private bool ResultPredicate(IRestResponse Response)
        {
            return Response.StatusCode != HttpStatusCode.OK;
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public IMessage Resolve(IMessage Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public async Task<byte[]> ResolveGetAsync(byte[] Query)
        {
            //Replace "==" due to parsing bug in RestSharp
            var Message = Convert.ToBase64String(Query).Replace("==", "");

            //Random Number to avoid Cache Servers
            var Request = new RestRequest($"dns-query?dns={Message}&r={Random.Next(0, 99999999)}");

            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecuteGetTaskAsync(Request));

            if (Response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"HTTP Status {Enum.GetName(typeof(HttpStatusCode), Response.StatusCode)}");

            return Response.RawBytes;
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            var Request = new RestRequest("dns-query");
            
            Request.AddParameter("application/dns-message", Query, "application/dns-message", ParameterType.RequestBody);
            
            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecutePostTaskAsync(Request));

            if (Response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"HTTP Status {Enum.GetName(typeof(HttpStatusCode), Response.StatusCode)}");

            return Response.RawBytes;
        }

        public async Task<IMessage> ResolveAsync(IMessage Message)
        {
            var RequestBytes = await BinarySerializer.SerializeAsync(Message);

            var ResponseBytes = await ResolveAsync(RequestBytes);

            return await BinarySerializer.DeserializeAsync<Message>(ResponseBytes);
        }

        private bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            //return SslPolicyErrors == SslPolicyErrors.None && Certificate.GetPublicKeyString() == PublicKey;
            return true;
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
                RestClient.ClearHandlers();
            }

            IsDisposed = true;
        }

        ~HTTPs()
        {
            Dispose(false);
        }
    }
}

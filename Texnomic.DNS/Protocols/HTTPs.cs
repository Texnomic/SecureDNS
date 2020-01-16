using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RestSharp;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Options;

namespace Texnomic.DNS.Protocols
{
    /// <summary>
    /// DNS Over HTTPS <see href="https://tools.ietf.org/html/rfc8484">(DoH)</see>
    /// </summary>
    public class HTTPs : IProtocol
    {
        private readonly HTTPsOptions Options;
        private readonly RestClient RestClient;
        private readonly BinarySerializer BinarySerializer;
        private readonly AsyncRetryPolicy<IRestResponse> RetryPolicy;
        private readonly Random Random;

        public HTTPs(IOptionsMonitor<HTTPsOptions> HTTPsOptions)
        {
            Options = HTTPsOptions.CurrentValue;

            Random = new Random();

            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;

            BinarySerializer = new BinarySerializer();

            RestClient = new RestClient(Options.Uri);

            RestClient.AddDefaultHeader("Cache-Control", "no-cache");

            RestClient.AddDefaultHeader("Accept", "application/dns-message");

            RestClient.Proxy = Options.WebProxy;

            RestClient.FollowRedirects = Options.AllowRedirects;

            RetryPolicy = Policy.HandleResult<IRestResponse>(ResultPredicate)
                                .RetryAsync(Options.Retries);
        }

        private static bool ResultPredicate(IRestResponse Response)
        {
            return Response.ErrorException != null;
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

            if (Response.ErrorException != null) throw Response.ErrorException;

            return Response.RawBytes;
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            var Request = new RestRequest("dns-query");

            Request.AddParameter("application/dns-message", Query, "application/dns-message", ParameterType.RequestBody);

            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecutePostTaskAsync(Request));

            if (Response.ErrorException != null) throw Response.ErrorException;

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
            return Options.PublicKey == null ? SslPolicyErrors == SslPolicyErrors.None : SslPolicyErrors == SslPolicyErrors.None && Certificate.GetPublicKeyString() == Options.PublicKey;
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

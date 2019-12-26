using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IPAddress IPAddress;
        private readonly RestClient RestClient;
        private readonly AsyncRetryPolicy<IRestResponse> RetryPolicy;
        private readonly SemaphoreSlim Semaphore;
        private readonly Random Random;

        public HTTPs(IPAddress IPAddress, string PublicKey)
        {
            this.IPAddress = IPAddress;
            this.PublicKey = PublicKey;

            Random = new Random();

            RestClient = new RestClient($"https://{IPAddress}/");
            //RestClient.FollowRedirects = false; //Google will fail
            RestClient.AddDefaultHeader("Cache-Control", "no-cache");
            RestClient.AddDefaultHeader("Accept", "application/dns-message");

            //RestClient.Proxy = new WebProxy("127.0.0.1:8888");

            RetryPolicy = Policy.HandleResult<IRestResponse>(ResultPredicate)
                                .RetryAsync(3);

            Semaphore = new SemaphoreSlim(1, 1);
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

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            //Replace "==" due to parsing bug in RestSharp
            var Message = Convert.ToBase64String(Query).Replace("==","");

            //Random Number to avoid Cache Servers
            var Request = new RestRequest($"dns-query?dns={Message}&r={Random.Next(0, 99999999)}")
            {
                Timeout = 500
            };

            //var Response = await RestClient.ExecuteGetTaskAsync(Request);

            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecuteGetTaskAsync(Request));

            if (Response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"HTTP Status {Enum.GetName(typeof(HttpStatusCode), Response.StatusCode)}");
            
            return Response.RawBytes;
        }

        public async Task<byte[]> ResolvePostAsync(byte[] Query)
        {
            var Request = new RestRequest("dns-query");
            Request.AddParameter("application/dns-message", Query, "application/dns-message", ParameterType.RequestBody);
            var Response = await RestClient.ExecutePostTaskAsync(Request);
            return Response.RawBytes;
        }

        public async Task<IMessage> ResolveAsync(IMessage Query)
        {
            var Request = new RestRequest($"resolve?name={Query.Questions[0].Domain}&type={Query.Questions[0].Type}");
            return await RestClient.GetAsync<Message>(Request);
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
                Semaphore.Dispose();
            }

            IsDisposed = true;
        }

        ~HTTPs()
        {
            Dispose(false);
        }
    }
}

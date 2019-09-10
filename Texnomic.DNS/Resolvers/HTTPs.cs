using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    /// <summary>
    /// DNS Over HTTPS <see href="https://tools.ietf.org/html/rfc8484">(DoH)</see>
    /// </summary>
    public class HTTPs : IResolver
    {
        private readonly string PublicKey;
        private readonly IPAddress IPAddress;
        private readonly RestClient RestClient;
        private readonly SemaphoreSlim Semaphore;

        public HTTPs(IPAddress IPAddress, string PublicKey)
        {
            this.IPAddress = IPAddress;
            this.PublicKey = PublicKey;

            RestClient = new RestClient($"https://{IPAddress}/");
            Semaphore = new SemaphoreSlim(1, 1);
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public Message Resolve(Message Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            var Message = Convert.ToBase64String(Query);
            var Request = new RestRequest($"dns-query?dns={Message}");
            Request.AddHeader("Accept", "application/dns-message");
            var Response = await RestClient.ExecuteGetTaskAsync(Request);
            if (Response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"HTTP Status {Enum.GetName(typeof(HttpStatusCode), Response.StatusCode)}");
            return Response.RawBytes;
        }

        public async Task<byte[]> ResolvePostAsync(byte[] Query)
        {
            var Request = new RestRequest("dns-query");
            Request.AddHeader("Accept", "application/dns-message");
            Request.AddParameter("application/dns-message", Query, "application/dns-message", ParameterType.RequestBody);
            var Response = await RestClient.ExecutePostTaskAsync(Request);
            return Response.RawBytes;
        }

        public async Task<Message> ResolveAsync(Message Query)
        {
            var Request = new RestRequest($"resolve?name={Query.Questions[0].Domain}&type={Query.Questions[0].Type}");
            Request.AddHeader("Accept", "application/dns-json");
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

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public class DoH : IResolver
    {
        private readonly string PublicKey;
        private readonly IPAddress IPAddress;
        private readonly RestClient RestClient;
        private readonly SemaphoreSlim Semaphore;

        public DoH(IPAddress IPAddress, string PublicKey)
        {
            this.IPAddress = IPAddress;
            this.PublicKey = PublicKey;

            RestClient = new RestClient($"https://{IPAddress}/");
            Semaphore = new SemaphoreSlim(1, 1);
        }

        public byte[] Resolve(byte[] Query)
        {
            throw new NotImplementedException();
        }

        public Message Resolve(Message Query)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            var Message = Convert.ToBase64String(Query);
            var Request = new RestRequest($"dns-query?dns={Message}");
            Request.AddHeader("Accept", "application/dns-message");
            var Response = await RestClient.ExecuteGetTaskAsync(Request);
            return Response.RawBytes;
        }

        public async ValueTask<Message> ResolveAsync(Message Query)
        {
            var Request = new RestRequest($"resolve?name={Query.Questions[0].Domain}&type={Query.Questions[0].Type}");
            Request.AddHeader("Accept", "application/dns-json");
            return await RestClient.GetAsync<Message>(Request);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RestSharp;
using Texnomic.SecureDNS.Core.Options;

namespace Texnomic.SecureDNS.Protocols
{
    /// <summary>
    /// DNS Over HTTPS <see href="https://tools.ietf.org/html/rfc8484">(DoH)</see>
    /// </summary>
    public class HTTPs : Protocol
    {
        private readonly IOptionsMonitor<HTTPsOptions> Options;
        private readonly RestClient RestClient;
        private readonly AsyncRetryPolicy<IRestResponse> RetryPolicy;

        public HTTPs(IOptionsMonitor<HTTPsOptions> HTTPsOptions)
        {
            Options = HTTPsOptions;

            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;

            RestClient = new RestClient();

            RestClient.AddDefaultHeader("Cache-Control", "no-cache");

            RestClient.AddDefaultHeader("Accept", "application/dns-message");

            RestClient.Proxy = Options.CurrentValue.WebProxy;

            RestClient.FollowRedirects = Options.CurrentValue.AllowRedirects;

            RetryPolicy = Policy.HandleResult<IRestResponse>(ResultPredicate)
                                .RetryAsync(Options.CurrentValue.Retries);
        }

        private static bool ResultPredicate(IRestResponse Response)
        {
            return Response.ErrorException != null;
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            var Request = new RestRequest(new Uri(Options.CurrentValue.Uri, "/dns-query"), Method.POST);

            Request.AddParameter("application/dns-message", Query, "application/dns-message", ParameterType.RequestBody);

            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecutePostAsync(Request));

            if (Response.ErrorException != null) 
                throw Response.ErrorException;

            return Response.RawBytes;
        }

        private bool ValidateServerCertificate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            return string.IsNullOrEmpty(Options.CurrentValue.PublicKey) ? SslPolicyErrors == SslPolicyErrors.None : SslPolicyErrors == SslPolicyErrors.None && Certificate.GetPublicKeyString() == Options.CurrentValue.PublicKey;
        }


        protected override void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                RestClient.ClearHandlers();
            }

            IsDisposed = true;
        }
    }
}

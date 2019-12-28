using System.Net;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class CloudflareHTTPsMiddleware : HTTPsMiddleware
    {
        public CloudflareHTTPsMiddleware() : base(IPAddress.Parse("1.1.1.1"), "")
        {

        }
    }
}

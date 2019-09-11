using System.Net;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class GoogleUDPMiddleware : UDPMiddleware
    {
        public GoogleUDPMiddleware() : base(IPAddress.Parse("8.8.8.8"))
        {
        }
    }
}

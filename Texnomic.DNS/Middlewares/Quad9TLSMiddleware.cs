using System.Net;

namespace Texnomic.DNS.Middlewares
{
    public class Quad9TLSMiddleware : TLSMiddleware
    {
        public Quad9TLSMiddleware() : base(IPAddress.Parse("9.9.9.9"), "047D8BD71D03850D1825B3341C29A127D4AC0125488AA0F1EA02B9D8512C086AAC7256ECFA3DA6A09F4909558EACFEB973175C02FB78CC2491946F4323890E1D66")
        {
        }
    }
}

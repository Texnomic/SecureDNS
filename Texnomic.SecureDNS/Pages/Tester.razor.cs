using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Texnomic.DNS;
using Texnomic.SecureDNS.Resolvers;

namespace Texnomic.SecureDNS.Pages
{
    public class TesterBase : ComponentBase
    {
        [Inject]
        protected DnsServer<DnsOverTls> DnsServer { get; set; }

        protected string Input { get; set; } = "facebook.com";

        protected string Output;

        protected async Task ResolveAsync()
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }
    }
}

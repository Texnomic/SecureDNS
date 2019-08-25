using Microsoft.AspNetCore.Components;
using Texnomic.SecureDNS.Services;

namespace Texnomic.SecureDNS.Pages
{
    public class ServerBase : ComponentBase
    {
        [Inject]
        protected DnsService DnsService { get; set; }

        protected string Output;

        protected void Start()
        {
            DnsService.Start();

            Output = "DNS Server Started Successfully.";
        }

        protected void Stop()
        {
            DnsService.Stop();

            Output = "DNS Server Stopped Successfully.";
        }
    }
}

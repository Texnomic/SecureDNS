using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;

namespace Texnomic.SecureDNS.Pages
{
    public class ProtocolsBase : ComponentBase
    {
        [Inject]
        protected IEnumerable<IHostedService> HostedServices { get; set; }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
    }
}

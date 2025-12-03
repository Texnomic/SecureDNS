using Microsoft.AspNetCore.Components;

namespace Texnomic.SecureDNS.Pages;

public class ProtocolsBase : ComponentBase
{
    [Inject]
    protected IEnumerable<IHostedService> HostedServices { get; set; }

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }
}
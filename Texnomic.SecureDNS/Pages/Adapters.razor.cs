using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Telerik.Blazor.Components.Grid;
using Texnomic.SecureDNS.Services;
using Texnomic.SecureDNS.Models;

namespace Texnomic.SecureDNS.Pages
{
    public class AdaptersBase : ComponentBase
    {
        [Inject]
        protected AdaptersService AdaptersService { get; set; }

        protected string Output;

        protected async Task SetDNSAsync(GridCommandEventArgs args)
        {
            var Adapter = (args.Item as Adapter);

            await AdaptersService.SetDNSAsync(Adapter);

            Output = "Network Adapters DNS Set Sucessfully.";
        }
        protected async Task ResetDNSAsync(GridCommandEventArgs args)
        {
            var Adapter = (args.Item as Adapter);

            await AdaptersService.ResetDNSAsync(Adapter);

            Output = "Network Adapters DNS Reset Sucessfully.";
        }
    }
}

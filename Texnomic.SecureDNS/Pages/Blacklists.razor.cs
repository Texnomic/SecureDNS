using Texnomic.DNS.Client;
using Texnomic.DNS.Server;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Services;
using Texnomic.SecureDNS.Data;

namespace Texnomic.SecureDNS.Pages
{
    public class BlacklistsBase : ComponentBase
    {
        [Inject]
        protected BlacklistsService BlacklistsService { get; set; }

        [Inject]
        protected DatabaseContext DatabaseContext { get; set; }

        protected string Output;

        protected async Task Initalize()
        {
            await BlacklistsService.Initalize();

            Output = "Blacklists Initalized Sucessfully.";
        }
    }
}

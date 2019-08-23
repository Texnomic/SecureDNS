using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Models;
using Texnomic.SecureDNS.Services;

namespace Texnomic.SecureDNS.Pages
{
    public class BlacklistsBase : ComponentBase
    {
        [Inject]
        protected BlacklistsService BlacklistsService { get; set; }

        [Inject]
        protected DatabaseContext DatabaseContext { get; set; }

        protected string Output;

        protected IEnumerable<Blacklist> Blacklists => DatabaseContext.Blacklists.Take(50).ToList();


        protected async Task InitializeAsync()
        {
            await BlacklistsService.InitializeAsync();

            Output = "Blacklists Initialized Successfully.";
        }
    }
}

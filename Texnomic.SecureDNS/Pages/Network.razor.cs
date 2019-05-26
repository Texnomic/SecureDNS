using Texnomic.DNS.Client;
using Texnomic.DNS.Server;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Services;
using Texnomic.SecureDNS.Data;

namespace Texnomic.SecureDNS.Pages
{
    public class NetworkBase : ComponentBase
    {
        [Inject]
        protected NetworkService NetworkService { get; set; }
    }
}

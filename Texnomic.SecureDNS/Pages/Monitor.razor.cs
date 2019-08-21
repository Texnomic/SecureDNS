using Microsoft.AspNetCore.Components;
using System;
using Texnomic.SecureDNS.Services;

namespace Texnomic.SecureDNS.Pages
{
    public class MonitorBase : ComponentBase
    {
        [Inject]
        public MonitorService MonitorService { get; set; }

        protected override void OnInitialized()
        {
            MonitorService.DataReceived += MonitorService_DataReceived;
            base.OnInitialized();
        }

        private void MonitorService_DataReceived(object Sender, EventArgs Args)
        {
            InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }
}

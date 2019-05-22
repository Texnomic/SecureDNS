using Microsoft.AspNetCore.Components;
using System;
using Texnomic.SecureDNS.Services;

namespace Texnomic.SecureDNS.Pages
{
    public class MonitorBase : ComponentBase
    {
        [Inject]
        public MonitorService MonitorService { get; set; }

        protected override void OnInit()
        {
            MonitorService.DataReceived += MonitorService_DataReceived;
            base.OnInit();
        }

        private void MonitorService_DataReceived(object sender, EventArgs e)
        {
            Invoke(StateHasChanged);
        }
    }
}

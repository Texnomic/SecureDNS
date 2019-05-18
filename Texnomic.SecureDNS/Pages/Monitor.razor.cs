using DNS.Client;
using DNS.Server;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Texnomic.SecureDNS.Data;

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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Invoke(StateHasChanged);
        }

        private void MonitorService_DataReceived(object sender, EventArgs e)
        {
            Invoke(StateHasChanged);
        }
    }
}

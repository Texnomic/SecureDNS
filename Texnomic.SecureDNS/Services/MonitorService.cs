using Texnomic.DNS.Protocol;
using Texnomic.DNS.Server;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.Collections.Generic;
using Texnomic.SecureDNS.Models;

namespace Texnomic.SecureDNS.Services
{
    public class MonitorService
    {
        private readonly DnsServer Server;

        public ObservableCollection<Query> Queries;

        public event EventHandler<EventArgs> DataReceived;

        public MonitorService(DnsServer Server)
        {
            this.Server = Server;
            Queries = new ObservableCollection<Query>();
            Server.Responded += Server_Responded;
        }

        private void Server_Responded(object Sender, DnsServer.RespondedEventArgs Args)
        {
            Queries.Add(new Query
            {
                IPEndPoint = Args.Remote,
                Request = Args.Request,
                Response = Args.Response
            });

            DataReceived?.Invoke(this, Args);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using Texnomic.DNS;
using Texnomic.SecureDNS.Models;
using Texnomic.SecureDNS.Resolvers;

namespace Texnomic.SecureDNS.Services
{
    public class MonitorService
    {
        private readonly DnsServer<DnsOverTls> Server;

        public ObservableCollection<Query> Queries;

        public event EventHandler<EventArgs> DataReceived;

        public MonitorService(DnsServer<DnsOverTls> Server)
        {
            this.Server = Server;
            Queries = new ObservableCollection<Query>();
            Server.Resolved += Server_Resolved;
        }

        private void Server_Resolved(object sender, DnsServer<DnsOverTls>.ResolvedEventArgs Args)
        {
            Queries.Add(new Query
            {
                IPEndPoint = Args.EndPoint,
                Request = Args.Request,
                Response = Args.Response
            });

            DataReceived?.Invoke(this, Args);
        }
    }
}

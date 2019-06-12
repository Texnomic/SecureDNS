using System;
using System.Threading.Tasks;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;
using Texnomic.SecureDNS.Data;

namespace Texnomic.SecureDNS.Resolvers
{
    public class DnsOverHttps : IResolver
    {
        private readonly DatabaseContext DatabaseContext;

        public DnsOverHttps(DatabaseContext DatabaseContext)
        {
            this.DatabaseContext = DatabaseContext;
        }

        public Task<Message> ResolveAsync(Message Request)
        {
            throw new NotImplementedException();
        }
    }
}

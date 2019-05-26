using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Texnomic.DNS.Protocol;
using Texnomic.DNS.Protocol.RequestResolvers;
using Texnomic.SecureDNS.Data;

namespace Texnomic.SecureDNS.Resolvers
{
    public class DnsOverHttps : IRequestResolver
    {
        private readonly DatabaseContext DatabaseContext;

        public DnsOverHttps(DatabaseContext DatabaseContext)
        {
            this.DatabaseContext = DatabaseContext;
        }

        public Task<IResponse> Resolve(IRequest Request)
        {
            throw new NotImplementedException();
        }
    }
}

using Hangfire;
using System;

namespace Texnomic.SecureDNS.Hangfire
{
    public class HangfireJobActivator : JobActivator
    {
        private readonly IServiceProvider ServiceProvider;

        public HangfireJobActivator(IServiceProvider ServiceProvider)
        {
            this.ServiceProvider = ServiceProvider;
        }

        public override object ActivateJob(Type Type)
        {
            return ServiceProvider.GetService(Type);
        }
    }
}

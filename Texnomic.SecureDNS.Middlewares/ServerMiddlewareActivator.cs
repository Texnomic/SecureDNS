using System;
using PipelineNet.MiddlewareResolver;

namespace Texnomic.SecureDNS.Middlewares
{
    public class ServerMiddlewareActivator : IMiddlewareResolver
    {
        private readonly IServiceProvider ServiceProvider;

        public ServerMiddlewareActivator(IServiceProvider ServiceProvider)
        {
            this.ServiceProvider = ServiceProvider;
        }

        public object Resolve(Type Type)
        {
            return ServiceProvider.GetService(Type);
        }
    }
}

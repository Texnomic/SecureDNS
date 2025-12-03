namespace Texnomic.SecureDNS.Middlewares;

public class ServerMiddlewareActivator(IServiceProvider ServiceProvider) : IMiddlewareResolver
{
    public MiddlewareResolverResult Resolve(Type Type)
    {
        return new MiddlewareResolverResult()
        {
            Middleware = ServiceProvider.GetService(Type)
        };
    }
}
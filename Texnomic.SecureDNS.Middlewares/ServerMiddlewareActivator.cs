namespace Texnomic.SecureDNS.Middlewares;

public class ServerMiddlewareActivator(IServiceProvider ServiceProvider) : IMiddlewareResolver
{
    public object Resolve(Type Type)
    {
        return ServiceProvider.GetService(Type);
    }
}
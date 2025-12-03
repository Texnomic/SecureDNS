using PipelineNet.Middleware;
<<<<<<< HEAD
using Texnomic.SecureDNS.Abstractions;
=======
>>>>>>> 80e159a06224c769a7805e12328fb284df6c8bc1

namespace Texnomic.SecureDNS.Servers.Proxy.Options;

public class ProxyResponsibilityChainOptions
{
    public List<string> Middlewares { get; set; }

    public List<Type> GetMiddlewares()
    {
        var Types = AppDomain.CurrentDomain.GetAssemblies()
            .Single(Assembly => Assembly.FullName.StartsWith("Texnomic.SecureDNS.Middlewares"))
            .GetTypes()
            .Where(Type => Type.GetInterfaces().Contains(typeof(IAsyncMiddleware<IMessage, IMessage>)))
            .ToLookup(Type => Type.Name, Type => Type);

        return Middlewares.SelectMany(Middleware => Types[Middleware]).ToList();
    }
}
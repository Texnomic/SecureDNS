using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Middlewares.Options;

public class MasterFileMiddlewareOptions
{
    public List<IAnswer> Answers { get; set; }
}
namespace Texnomic.SecureDNS.Middlewares.Options;

public class ResolverMiddlewareOptions
{
    public bool CacheEnabled { get; set; } = true;
    public double CacheCompactPercentage { get; set; } = 50.0;
    public int CacheCompactTimeout { get; set; } = 24 * 60 * 60 * 1000;
    public int CacheStatusTimeout { get; set; } = 1 * 60 * 60 * 1000;

}
namespace Texnomic.SecureDNS.Protocols.Options;

public class HTTPsOptions : IOptions
{
    public Uri Uri { get; set; }

    public WebProxy WebProxy { get; set; }
}
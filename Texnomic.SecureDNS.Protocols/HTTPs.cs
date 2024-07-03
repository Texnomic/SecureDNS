using System.Net.Http.Headers;


namespace Texnomic.SecureDNS.Protocols;

/// <summary>
/// DNS Over HTTPS <see href="https://tools.ietf.org/html/rfc8484">(DoH)</see>
/// </summary>
public class HTTPs : Protocol
{
    private readonly IOptionsMonitor<HTTPsOptions> Options;

    private readonly HttpClient HttpClient;

    private readonly MediaTypeHeaderValue ContentType;

    public HTTPs(IOptionsMonitor<HTTPsOptions> HTTPsOptions)
    {
        Options = HTTPsOptions;

        var Handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        };

        HttpClient = new HttpClient(Handler)
        {
            BaseAddress = new Uri($"{Options.CurrentValue.Uri}/dns-query")
        };

        HttpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

        HttpClient.DefaultRequestHeaders.Add("Accept", "application/dns-message");

        ContentType = new MediaTypeHeaderValue("application/dns-message");

        if (Options.CurrentValue.WebProxy is not null)
            HttpClient.DefaultProxy = Options.CurrentValue.WebProxy;
    }

    public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
    {
        var Content = new ByteArrayContent(Query);

        Content.Headers.ContentType = ContentType;

        var Response = await HttpClient.PostAsync(string.Empty, Content);

        Response.EnsureSuccessStatusCode();

        return await Response.Content.ReadAsByteArrayAsync();
    }

    protected override void Dispose(bool Disposing)
    {
        if (IsDisposed) return;

        if (Disposing)
        {
            HttpClient.Dispose();
        }

        IsDisposed = true;
    }
}
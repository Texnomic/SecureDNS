using System.Net;

namespace Texnomic.SecureDNS.Extensions;

public static class IPEndPointExtensions
{
    public static IPEndPoint Parse(this IPEndPoint IPEndPoint, string Text)
    {
        if (Uri.TryCreate(Text, UriKind.Absolute, out var uri))
            return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);

        if (Uri.TryCreate(string.Concat("tcp://", Text), UriKind.Absolute, out uri))
            return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);

        if (Uri.TryCreate(string.Concat("tcp://", $"[{Text}]"), UriKind.Absolute, out uri))
            return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);

        throw new FormatException("Failed To Parse Text To IPEndPoint.");
    }
}
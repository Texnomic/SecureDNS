using System;
using System.Net;

namespace Texnomic.DNS.Extensions
{
    public static class IPEndPointExtensions
    {
        public static IPEndPoint Parse(this IPEndPoint IPEndPoint, string text)
        {
            if (Uri.TryCreate(text, UriKind.Absolute, out Uri uri))
                return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);

            if (Uri.TryCreate(string.Concat("tcp://", text), UriKind.Absolute, out uri))
                return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);

            if (Uri.TryCreate(string.Concat("tcp://", string.Concat("[", text, "]")), UriKind.Absolute, out uri))
                return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);

            throw new FormatException("Failed To Parse Text To IPEndPoint.");
        }
    }
}

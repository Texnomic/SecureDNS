using System;
using System.Net;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Protocols.Options;

public class HTTPsOptions : IOptions
{
    public Uri Uri { get; set; }

    public WebProxy WebProxy { get; set; }
}
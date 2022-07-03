using System;
using System.Collections.Generic;
using System.Net;
using YamlDotNet.Serialization;

namespace Texnomic.DNS.Roots.Models;

public class Root
{
    public string Operator { get; set; }

    public IPAddress IPv4 { get; set; }

    public IPAddress IPv6 { get; set; }

    public string ASN { get; set; }

    public Uri Homepage { get; set; }

    public Uri Statistics { get; set; }

    [YamlMember(Alias = "Peering Policy")]
    public string PeeringPolicy { get; set; }

    [YamlMember(Alias = "Contact Email")]
    public string ContactEmail { get; set; }

    public Uri RSSAC { get; set; }

    [YamlMember(Alias = "Identifier Naming Convention")]
    public string IdentifierNamingConvention { get; set; }

    public List<Instance> Instances { get; set; }
}
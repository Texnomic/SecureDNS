using System.Net;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Data.Models;

public class Resolver
{
    public int ID { get; set; }
    public string Name { get; set; }
    public IDomain Domain { get; set; }
    public IPAddress IPAddress { get; set; }
    public Hexadecimal Hash { get; set; }
    public bool UDP { get; set; }
    public bool TCP { get; set; }
    public bool TLS { get; set; }
    public bool HTTPS { get; set; }
    public bool CRYPT { get; set; }
}
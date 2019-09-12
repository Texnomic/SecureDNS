using System.Net;
using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface IIPv6Address
    {
        [Ignore] 
        IPAddress Value { get; set; }
    }
}

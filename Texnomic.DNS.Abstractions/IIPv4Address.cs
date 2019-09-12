using System.Net;
using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface IIPv4Address
    {
        [Ignore] 
        IPAddress Value { get; set; }
    }
}

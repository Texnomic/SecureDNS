namespace Texnomic.SecureDNS.Protocols.Options;

public class TCPOptions : IOptions
{
    public string IPv4Address { get; set; }

    public int Port { get; set; }

    public IPEndPoint IPv4EndPoint => new IPEndPoint(IPAddress.Parse(IPv4Address), Port);

    public TimeSpan Timeout { get; set; }

}
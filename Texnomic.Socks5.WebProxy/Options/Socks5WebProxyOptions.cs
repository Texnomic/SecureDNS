namespace Texnomic.Socks5.WebProxy.Options;

public class Socks5WebProxyOptions
{
    public bool Enabled { get; set; } = false;
    public IPAddress IPAddress { get; set; } = IPAddress.Parse("127.0.0.1");
    public int Port { get; set; } = 8000;
    public int BackLog { get; set; } = 8;
    public int Timeout { get; set; } = 2000;
    internal IPEndPoint IPEndPoint => new IPEndPoint(IPAddress, Port);
    public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
    public SocketType SocketType { get; set; } = SocketType.Stream;
}
using System.Net;
using System.Net.Sockets;
using Texnomic.Socks5.Enum;

namespace Texnomic.Socks5.Options;

public class Socks5Options
{
    public IPEndPoint IPEndPoint { get; set; } = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
    public Authentication Authentication = Authentication.NoAuthentication;
    public string Username { get; set; }
    public string Password { get; set; }
    public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
    public SocketType SocketType { get; set; } = SocketType.Stream;
}
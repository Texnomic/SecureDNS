namespace Texnomic.SecureDNS.Servers.Proxy.Options;

public class ProxyServerOptions
{
    public string Address { get; set; } = "127.0.0.1";

    public int Port { get; set; } = 53;

    [JsonIgnore]
    public IPEndPoint IPEndPoint => new(IPAddress.Parse(Address), Port);

    public int Threads { get; set; } = Environment.ProcessorCount;
}
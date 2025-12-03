<<<<<<< HEAD
﻿using System.Net;
using System.Text.Json.Serialization;
=======
﻿namespace Texnomic.SecureDNS.Servers.Proxy.Options;
>>>>>>> 80e159a06224c769a7805e12328fb284df6c8bc1

public class ProxyServerOptions
{
    public string Address { get; set; } = "127.0.0.1";

    public int Port { get; set; } = 53;

    [JsonIgnore]
    public IPEndPoint IPEndPoint => new(IPAddress.Parse(Address), Port);

    public int Threads { get; set; } = Environment.ProcessorCount;
}
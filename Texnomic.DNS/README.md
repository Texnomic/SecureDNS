﻿# DNS

DNS Protocol C# Library with Client and Server Implementation.

# Credits
1. [Mirza Kapetanovic](https://github.com/kapetan/dns) for DNS Protocol Original Implementation.
2. [Justin Garfield](https://github.com/justingarfield/dns-over-tls) for DNS-Over-TLS Original Implementation.

# Usage

The library exposes a `Request` and `Response` classes for parsing and creating DNS messages. These can be serialized to byte arrays.

```C#
Request request = new Request();

request.RecursionDesired = true;
request.Id = 123;

UdpClient udp = new UdpClient();
IPEndPoint google = new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53);

// Send to google's DNS server
await udp.SendAsync(request.ToArray(), request.Size, google);

UdpReceiveResult result = await udp.ReceiveAsync();
byte[] buffer = result.Buffer;
Response response = Response.FromArray(buffer);

// Outputs a human readable representation
Console.WriteLine(response);
```

### Client

The libray also includes a small client and a proxy server. Using the `ClientRequest` or the `DnsClient` class it is possible to send a request to a Domain Name Server. The request is first sent using UDP, if that fails (response is truncated), the request is sent again using TCP. This behaviour can be changed by supplying an `IRequestResolver` to the client constructor.

```C#
ClientRequest request = new ClientRequest("8.8.8.8");

// Request an IPv6 record for the foo.com domain
request.Questions.Add(new Question(Domain.FromString("foo.com"), RecordType.AAAA));
request.RecursionDesired = true;

ClientResponse response = await request.Resolve();

// Get all the IPs for the foo.com domain
IList<IPAddress> ips = response.AnswerRecords
	.Where(r => r.Type == RecordType.AAAA)
	.Cast<IPAddressResourceRecord>()
	.Select(r => r.IPAddress)
	.ToList();
```

The `DnsClient` class contains some conveniance methods for creating instances of `ClientRequest` and resolving domains.

```C#
// Bind to a Domain Name Server
DnsClient client = new DnsClient("8.8.8.8");

// Create request bound to 8.8.8.8
ClientRequest request = client.Create();

// Returns a list of IPs
IList<IPAddress> ips = await client.Lookup("foo.com");

// Get the domain name belonging to the IP (google.com)
string domain = await client.Reverse("173.194.69.100");
```

### Server

The `DnsServer` class exposes a proxy Domain Name Server (UDP only). You can intercept domain name resolution requests and route them to specified IPs. The server is asynchronous. It also emits an event on every request and every successful resolution.

```C#
// Proxy to google's DNS
MasterFile masterFile = new MasterFile();
DnsServer server = new DnsServer(masterFile, "8.8.8.8");

// Resolve these domain to localhost
masterFile.AddIPAddressResourceRecord("google.com", "127.0.0.1");
masterFile.AddIPAddressResourceRecord("github.com", "127.0.0.1");

// Log every request
server.Requested += (sender, e) => Console.WriteLine(e.Request);
// On every successful request log the request and the response
server.Responded += (sender, e) => Console.WriteLine("{0} => {1}", e.Request, e.Response);
// Log errors
server.Errored += (sender, e) => Console.WriteLine(e.Exception.Message);

// Start the server (by default it listents on port 53)
await server.Listen();
```

Depending on the application setup the events might be executed on a different thread than the calling thread.

It's also possible to modify the `request` instance in the `server.Requested` callback.

### Request Resolver

The `DnsServer`, `DnsClient` and `ClientRequest` classes also accept an instance implementing the `IRequestResolver` interface, which they internally use to resolve DNS requests. Some of the default implementations are `UdpRequestResolver`, `TcpRequestResolver` and `MasterFile` classes. But it's also possible to provide a custom request resolver.

```C#
// A request resolver that resolves all dns queries to localhost
public class LocalRequestResolver : IRequestResolver {
	public Task<IResponse> Resolve(IRequest request) {
		IResponse response = Response.FromRequest(request);

		foreach (Question question in response.Questions) {
			if (question.Type == RecordType.A) {
				IResourceRecord record = new IPAddressResourceRecord(
					question.Name, IPAddress.Parse("127.0.0.1"));
				response.AnswerRecords.Add(record);
			}
		}

		return Task.FromResult(response);
	}
}

// All dns requests received will be handled by the localhost request resolver
DnsServer server = new DnsServer(new LocalRequestResolver());

await server.Listen();
```

# License

**This software is licensed under "MIT"**

> Copyright (c) 2019 Mohamed Samy (Texnomic)
>
> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
>
> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
>
> THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

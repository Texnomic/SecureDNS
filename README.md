![SecureDNS](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/GitHub/Logo.png "SecureDNS")

Building a Secure, Modern & Cross-Platform DNS Server Using C# 8.0 & .NET Core 3.0.

The SecureDNS Server will implement **all secure DNS Communication Protocols regardless of being standardized or not** while maintaining backward compatibility with the classic unsecure DNS Over UDP Protocol via Reverse Proxy.

The SecureDNS Server is implemented using modern [Clean-Architecture Patterns](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164).

# Implementations
1. DNS Over Blockchain ([Ethereum Name Service](https://ens.domains/) + DNS Resolver SmartContract)
2. [DNS Over UDP](https://tools.ietf.org/html/rfc1035)
3. [DNS Over TCP](https://tools.ietf.org/html/rfc1035)
4. [DNS Over TLS](https://tools.ietf.org/html/rfc7858)
5. [DNS Over HTTP/S](https://tools.ietf.org/html/rfc8484)
6. [DNSCrypt](https://dnscrypt.info/)

# Project Launch
The Project offically launched within my talk for [Cairo Security Camp](https://cairosecuritycamp.com/sessions/rebuilding-the-domain-name-system/) on September 22nd.

>[Rebuilding Domain Name System Presentation](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/GitHub/Rebuilding.DNS.pptx)

>This talk will cover the DNS protocol since its inception in 1986 and taking a deep-dive into the inherit lack of security at its core design and how all modern operating systems using insecure standards, Then we will explain alternative secure implementation like DNS Over TLS and DNS Over HTTP/S. Then we will shift-gears to developing an all-new modern DNS Server Reference-Implementation with a myriad of possibilities for retaking control like Black/Sink-holing, Threat Hunting/Intelligence and way more! Finally, we will launch a GitHub-based Open-Source Project containing DNS Server Reference-Implementation for the First-Time & Exclusively in Cairo Security Camp!

# Technology Stack
1. [C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)
2. [.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)
3. [ASP.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)
4. [PowerShell Core 7.0](https://github.com/PowerShell/PowerShell)
5. [Entity Framework Core 3.0](https://docs.microsoft.com/en-us/ef/core/)
6. [Blazor aka Razor Components](https://dotnet.microsoft.com/apps/aspnet/web-apps/client)


# Dependencies
1. [HangFire](https://www.hangfire.io/)
2. [MoreLINQ](https://github.com/morelinq/MoreLINQ)
3. [Electron.NET](https://github.com/ElectronNET/Electron.NET)
4. [BinarySerializer](https://github.com/jefffhaynes/BinarySerializer)
5. [FlexLabs.Upsert](https://github.com/artiomchi/FlexLabs.Upsert)
6. [Telerik UI for Blazor](https://www.telerik.com/blazor-ui)
7. [Hangfire Extension Plugins](https://github.com/wanlitao/HangfireExtension)
8. [Entity Framework Extensions](https://entityframework-extensions.net)


# Supported By
![JetBrains](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/GitHub/JetBrains.png "JetBrains")
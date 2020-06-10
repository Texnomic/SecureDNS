<div style="text-align:center"><img src="https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Logo.png" alt="SecureDNS" /></div>


Building a Secure, Modern, Cross-Platform & Cross-Architecture DNS Server Using C# 8.0 & .NET Core 3.1.

SecureDNS Project aims to implements all *secure* DNS Protocols regardless of being standardized or widely adopted; while maintaining backward compatibility with *unsecure* DNS-Over-UDP Protocol via Reverse Proxy.

SecureDNS Project is implemented using modern [Clean-Architecture](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164) Patterns.

# Table Of Content

1. [Releases](#Releases)
2. [Protocols](#Protocols)
3. [Integrations](#Integrations)
4. [Resource Records](https://github.com/Texnomic/SecureDNS/wiki/Supported-Resource-Records)
5. [Platforms](#Platforms)
6. [Architecture](#Architecture)
7. [Launch](#Launch)
8. [Technology](#Technology)
9. [Dependencies](#Dependencies)
10. [Donations](#Donations)
11. [Supported-By](#Supported-By)

## Releases

![GitHub Release](https://img.shields.io/github/v/release/Texnomic/SecureDNS?include_prereleases&label=GitHub%20Release%20|%20Terminal%20Edition&style=flat-square) 
![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Abstractions?label=NuGet%20%7C%20Texnomic.SecureDNS.Abstractions&style=flat-square) 
![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Core?label=NuGet%20%7C%20Texnomic.SecureDNS.Core&style=flat-square)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Protocols?label=NuGet%20%7C%20Texnomic.SecureDNS.Protocols&style=flat-square)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Serialization?label=NuGet%20%7C%20Texnomic.SecureDNS.Serialization&style=flat-square)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Texnomic.Sodium?label=NuGet%20%7C%20Texnomic.Sodium&style=flat-square)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Texnomic.FilterLists?label=NuGet%20%7C%20%09Texnomic.FilterLists&style=flat-square)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Texnomic.ENS.BaseRegistrar?label=NuGet%20%7C%20%09Texnomic.ENS.BaseRegistrar&style=flat-square)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Texnomic.ENS.PublicResolver?label=NuGet%20%7C%20%09Texnomic.ENS.PublicResolver&style=flat-square)

<div style="text-align:center"><a href="https://www.youtube.com/embed/24QwvJ1VTmQ"><img src="https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/YouTube.png" /></a></div>



## Protocols

- ✔ [DNS Over Blockchain (ENS)](https://ens.domains/)
- ✔ [DNS Over UDP](https://tools.ietf.org/html/rfc1035)
- ✔ [DNS Over TCP](https://tools.ietf.org/html/rfc1035)
- ✔ [DNS Over TLS](https://tools.ietf.org/html/rfc7858)
- ✔ [DNS Over HTTPs](https://tools.ietf.org/html/rfc8484)
- ✔ [DNSCrypt v2.0](https://dnscrypt.info/)
- ⏳ [DNS Over Blockchain (HandShake)](https://handshake.org/)
- ⏳ [DNS Over UDP Over Tor](https://tools.ietf.org/html/rfc1035)
- ⏳ [DNS Over TCP Over Tor](https://tools.ietf.org/html/rfc1035)
- ⏳ [DNS Over TLS Over Tor](https://tools.ietf.org/html/rfc7858)
- ⏳ [DNS Over HTTPs Over Tor](https://tools.ietf.org/html/rfc8484)
- ⏳ [DNSCrypt v2.0 Over Tor](https://dnscrypt.info/)

## Integrations

- ✔ [Ethereum Name Service](https://ens.domains/)
- ✔ [Ethereum Name Service DNS Resolver](https://github.com/ensdomains/resolvers)
- ✔ [FilterLists](https://github.com/collinbarrett/FilterLists)
- ✔ [Tor Project](https://www.torproject.org/)
- ✔ Standard-Compliant DNS Resolvers
- ⏳ [IANA Root Files](https://www.iana.org/domains/root/files)
- ⏳ [ICANN Zone Data Service](https://czds.icann.org/home)


## Platforms

- ✔ Alpine: 3.10+
- ✔ Debian: 9+
- ✔ Ubuntu: 16.04+
- ✔ Fedora: 29+
- ✔ RHEL: 6+
- ✔ openSUSE: 15+
- ✔ SUSE Enterprise Linux (SLES): 12 SP2+
- ✔ macOS: 10.13+
- ✔ Windows Client: 7, 8.1, 10 (1607+)
- ✔ Windows Server: 2012 R2+

## Architectures

- ✔ x64 on Windows, macOS, and Linux
- ✔ x86 on Windows, macOS, and Linux
- ✔ ARM32 on Windows and Linux
- ✔ ARM64 on Windows and Linux

## Launch

The Project offically launched within my talk for [Cairo Security Camp](https://cairosecuritycamp.com/sessions/rebuilding-the-domain-name-system/) on September 22nd, 2019.

>[Full Talk Video On YouTube](https://youtu.be/1Gxk40dmbFM)

>[Rebuilding Domain Name System Presentation](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Rebuilding.DNS.pptx)

>This talk will cover the DNS protocol since its inception in 1986 and taking a deep-dive into the inherit lack of security at its core design and how all modern operating systems using insecure standards, Then we will explain alternative secure implementation like DNS Over TLS and DNS Over HTTP/S. Then we will shift-gears to developing an all-new modern DNS Server Reference-Implementation with a myriad of possibilities for retaking control like Black/Sink-holing, Threat Hunting/Intelligence and way more! Finally, we will launch a GitHub-based Open-Source Project containing DNS Server Reference-Implementation for the First-Time & Exclusively in Cairo Security Camp!

## Technology

1. [C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)
2. [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.0)
3. [ASP.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.0)
4. [PowerShell Core 7.0](https://github.com/PowerShell/PowerShell)
5. [Entity Framework Core 3.1](https://docs.microsoft.com/en-us/ef/core/)
6. [Blazor aka Razor Components](https://dotnet.microsoft.com/apps/aspnet/web-apps/client)
7. [Solidity](https://github.com/ethereum/solidity)

## Dependencies

1. [HangFire](https://www.hangfire.io/)
2. [MoreLINQ](https://github.com/morelinq/MoreLINQ)
3. [Electron.NET](https://github.com/ElectronNET/Electron.NET)
4. [BinarySerializer](https://github.com/jefffhaynes/BinarySerializer)
5. [FlexLabs.Upsert](https://github.com/artiomchi/FlexLabs.Upsert)
6. [Telerik UI for Blazor](https://www.telerik.com/blazor-ui)
7. [Hangfire Extension Plugins](https://github.com/wanlitao/HangfireExtension)
8. [Entity Framework Extensions](https://entityframework-extensions.net)
9. [Nethereum](https://nethereum.com/)
10. [Polly](https://github.com/App-vNext/Polly)
11. [Terminal UI](https://github.com/migueldeicaza/gui.cs)
12. [Command Line Parser](https://github.com/commandlineparser/commandline)
13. [Async Enumerable](https://github.com/Dasync/AsyncEnumerable)
14. [Colorful Console](http://colorfulconsole.com/)
15. [Blazorise](https://blazorise.com/)
16. [HtmlAgilityPack](https://html-agility-pack.net/)
17. [LibSodium](https://github.com/jedisct1/libsodium)

## Donations

* **PayPal**: https://www.paypal.me/texnomic
* **Bitcoin**: 13wMqy8yg9yhJAAP2AXu8A2De1ptAYh6s4
* **Ethereum**: 0xfE171b1C5C5584b65ec58a6FA2009f6ECeE812D7

## Supported-By

![JetBrains](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/JetBrains.png "JetBrains")
![Syncfusion](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Syncfusion.png "Syncfusion")

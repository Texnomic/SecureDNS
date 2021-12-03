<div style="text-align:center"><img src="https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Logo.png" alt="SecureDNS" /></div>

[![Twitter Follow](https://img.shields.io/twitter/follow/Texnomic?color=black&logoColor=blue&style=social)](https://twitter.com/texnomic)
[![YouTube Video Views](https://img.shields.io/youtube/views/24QwvJ1VTmQ?label=YouTube%20%7C%20SecureDNS&style=social)](https://youtu.be/24QwvJ1VTmQ)
[![Slack](https://img.shields.io/static/v1?logo=Slack&label=Slack&message=Channel&color=73BA25&logoColor=4A154B)](https://join.slack.com/t/texnomicsecuredns/shared_invite/zt-h110u1u8-5VwSZsQSHL13s62xAZjI2Q)
[![Facebook](https://img.shields.io/static/v1?logo=Facebook&label=Facebook&message=Page&color=73BA25&logoColor=1877FF2)](https://www.facebook.com/Texnomic-Secure-DNS-114240320331170)

Building a Secure, Modern, Cross-Platform & Cross-Architecture DNS Server Using .NET 6.0.

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

- ![GitHub Release](https://img.shields.io/github/v/release/Texnomic/SecureDNS?logo=GitHub&include_prereleases&label=GitHub%20Release%20|%20Terminal%20Edition)
- ![GitHub Downloads](https://img.shields.io/github/downloads/Texnomic/SecureDNS/total?color=Orange&label=GitHub%20Downloads%20%7C%20Terminal%20Edition&logo=GitHub)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.Sodium?logo=NuGet&label=NuGet%20%7C%20Texnomic.Sodium&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.Socks5?logo=NuGet&label=NuGet%20%7C%20%09Texnomic.Socks5&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.FilterLists?logo=NuGet&label=NuGet%20%7C%20%09Texnomic.FilterLists&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.ENS.BaseRegistrar?logo=NuGet&label=NuGet%20%7C%20%09Texnomic.ENS.BaseRegistrar&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.ENS.PublicResolver?logo=NuGet&label=NuGet%20%7C%20%09Texnomic.ENS.PublicResolver&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Abstractions?logo=NuGet&label=NuGet%20%7C%20Texnomic.SecureDNS.Abstractions&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Core?logo=NuGet&label=NuGet%20%7C%20Texnomic.SecureDNS.Core&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Protocols?logo=NuGet&label=NuGet%20%7C%20Texnomic.SecureDNS.Protocols&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Serialization?logo=NuGet&label=NuGet%20%7C%20Texnomic.SecureDNS.Serialization&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Middlewares?logo=NuGet&label=NuGet%20%7C%20%09Texnomic.SecureDNS.Middlewares&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Servers?logo=NuGet&label=NuGet%20%7C%20%09Texnomic.SecureDNS.Servers&logoColor=blue&color=blue)
- ![NuGet](https://img.shields.io/nuget/vpre/Texnomic.SecureDNS.Extensions?logo=NuGet&label=NuGet%20%7C%20%09Texnomic.SecureDNS.Extensions&logoColor=blue&color=blue)

<div style="text-align:center"><a href="https://www.youtube.com/embed/24QwvJ1VTmQ"><img src="https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/YouTube.png" /></a><img src="https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/WinGet.png" /></div>

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

- ![Windows Client](https://img.shields.io/static/v1?logo=Windows&label=Windows%20Client&message=7%2C+8.1%2C+10+%281607%2B%29&color=0078D6&logoColor=0078D6)
- ![Windows Server](https://img.shields.io/static/v1?logo=Windows&label=Windows%20Server&message=2012%20R2%2B&color=0078D6&logoColor=0078D6)
- ![macOS](https://img.shields.io/static/v1?logo=Apple&label=macOS&message=10.13%2B&color=999999&logoColor=999999)
- ![Alpine](https://img.shields.io/static/v1?logo=Alpine%20Linux&label=Alpine%20Linux&message=3.10%2B&color=0D597F&logoColor=0D597F)
- ![Debian](https://img.shields.io/static/v1?logo=Debian&label=Debian&message=9%2B&color=A81D33&logoColor=A81D33)
- ![Ubuntu](https://img.shields.io/static/v1?logo=Ubuntu&label=Ubuntu&message=16.04%2B&color=E95420&logoColor=E95420)
- ![Fedora](https://img.shields.io/static/v1?logo=Fedora&label=Fedora&message=29%2B&color=294172&logoColor=294172)
- ![RHEL](https://img.shields.io/static/v1?logo=Red%20Hat&label=Red%20Hat%20Enterprise%20Linux&message=15%2B&color=EE0000&logoColor=EE0000)
- ![openSUSE](https://img.shields.io/static/v1?logo=openSUSE&label=openSUSE&message=15%2B&color=73BA25&logoColor=73BA25)
- ![SUSE](https://img.shields.io/static/v1?logo=openSUSE&label=SUSE%20Enterprise&message=12%20SP2%2B&color=73BA25&logoColor=73BA25)

## Architectures

- ✔ x64 on Windows, macOS, and Linux
- ✔ x86 on Windows, macOS, and Linux
- ✔ ARM32 on Windows and Linux
- ✔ ARM64 on Windows and Linux

## Launch

The Project offically launched within my talk for [Cairo Security Camp](https://cairosecuritycamp.com/sessions/rebuilding-the-domain-name-system/) on September 22nd, 2019.

[![YouTube Video Views](https://img.shields.io/youtube/views/1Gxk40dmbFM?label=YouTube%20%7C%20Cairo%20Security%20Camp%20Talk&style=social)](https://youtu.be/1Gxk40dmbFM)
[![PowerPoint](https://img.shields.io/static/v1?logo=Microsoft%20PowerPoint&label=Rebuilding%20Domain%20Name%20System&message=Presentation&color=B7472A&logoColor=B7472A)](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Rebuilding.DNS.pptx)

>This talk will cover the DNS protocol since its inception in 1986 and taking a deep-dive into the inherit lack of security at its core design and how all modern operating systems using insecure standards, Then we will explain alternative secure implementation like DNS Over TLS and DNS Over HTTP/S. Then we will shift-gears to developing an all-new modern DNS Server Reference-Implementation with a myriad of possibilities for retaking control like Black/Sink-holing, Threat Hunting/Intelligence and way more! Finally, we will launch a GitHub-based Open-Source Project containing DNS Server Reference-Implementation for the First-Time & Exclusively in Cairo Security Camp!

## Technology

1. [C# 8.0](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)
2. [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
3. [ASP.NET Core 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
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

* [![PayPal](https://img.shields.io/static/v1?logo=PayPal&label=PayPal&message=https://www.paypal.me/texnomic&color=blue)](https://www.paypal.me/texnomic)
* ![Bitcoin](https://img.shields.io/static/v1?logo=Bitcoin&label=BTC&message=13wMqy8yg9yhJAAP2AXu8A2De1ptAYh6s4&color=orange)
* ![Ethereum](https://img.shields.io/static/v1?logo=Ethereum&label=Ethereum&message=0xfE171b1C5C5584b65ec58a6FA2009f6ECeE812D7&color=black&logoColor=black)

## Supported-By

![JetBrains](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/JetBrains.png "JetBrains")
![Syncfusion](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Syncfusion.png "Syncfusion")
![AdvancedInstaller](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/AdvancedInstaller.png "AdvancedInstaller")

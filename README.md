![SecureDNS](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Logo.png "SecureDNS")

Building a Secure, Modern, Cross-Platform & Cross-Architecture DNS Server Using C# 8.0 & .NET Core 3.1.

SecureDNS Project aims to implements all *secure* DNS Protocols regardless of being standardized or widely adopted; while maintaining backward compatibility with *unsecure* DNS Over UDP Protocol via Reverse Proxy.

SecureDNS Project is implemented using modern [Clean-Architecture](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164) Patterns.

# Table Of Content
1. [Releases](#Releases)
2. [Protocols](#Protocols)
3. [Integrations](#Integrations)
4. [Records](#Records)
5. [Platforms](#Platforms)
6. [Chips](#Chips)
7. [Launch](#Launch)
8. [Technology](#Technology)
9. [Dependencies](#Dependencies)
10. [Donations](#Donations)
11. [Supported-By](#Supported-By)

## Releases
The [v0.2 Alpha Release](https://github.com/Texnomic/SecureDNS/releases/tag/v0.2-alpha) is now available.

## Protocols
- [x] [DNS Over Blockchain (ENS)](https://ens.domains/)
- [ ] [DNS Over Blockchain (HandShake)](https://handshake.org/)
- [ ] [DNS Over TCP](https://tools.ietf.org/html/rfc1035)
- [x] [DNS Over TLS](https://tools.ietf.org/html/rfc7858)
- [x] [DNS Over HTTP/S](https://tools.ietf.org/html/rfc8484)
- [ ] [DNSCrypt](https://dnscrypt.info/)

## Integrations
1. [Ethereum Name Service](https://ens.domains/)
2. [Ethereum Name Service DNS Resolver](https://github.com/ensdomains/resolvers)
3. [FilterLists](https://github.com/collinbarrett/FilterLists)
4. [IANA Root Files](https://www.iana.org/domains/root/files)
5. [ICANN Zone Data Service](https://czds.icann.org/home)
6. Any Standard-Compliant Public `DoH` or `DoT` Resolvers.

## Records
| Type | Code | Supported | Description | 
| :---: | :---: | :---:  | :--- |
| `A` | `1` | `Yes` | IPv4 Address |
| `NS` | `2` | `Yes` | Authoritative Name Server |
| `MD` | `3` | `Yes` | Mail Destination |
| `MF` | `4` | `Yes` | Mail Forwarder  |
| `CNAME` | `5` | `Yes` | Canonical Name for An Alias |
| `SOA` | `6` | `Yes` | Marks The Start of A Zone of Authority |
| `MB` | `7` | `Yes` | Mailbox Domain  |
| `MG` | `8` | `Yes` | Mail Group Member  |
| `MR` | `9` | `Yes` | Mail Rename Domain Name  |
| `NULL` | `10` | `Yes` | RFC 1035 |
| `WKS` | `11` | `Yes` | A Well Known Service Description  |
| `PTR` | `12` | `Yes` | Pointer Record  |
| `HINFO` | `13` | `Yes` | Host Information Resource Record  |
| `MX` | `15` | `Yes` | Mail Exchange  |
| `TXT` | `16` | `Yes` | Text Record  |
| `RP` | `17` | `No` | Responsible Person  |
| `AFSDB` | `18` | `No` | AFS Database Record  |
| `X25` | `19` | `No` | Public Switched Data Network (PSDN) Address Number |
| `ISDN` | `20` | `No` | Integrated Services Digital Network (ISDN) Telephone Number |
| `RT` | `21` | `No` | Route Through  |
| `NSAP` | `22` | `No` | Network Service Access Point (NSAP) Address  |
| `NSAP_PTR` | `23` | `No` | Network Service Access Point (NSAP) Pointer  |
| `SIG` | `24` | `No` | Signature Record  |
| `Key` | `25` | `No` | Key Record  |
| `PX` | `26` | `No` | MIXER Conformant Global Address Mapping  |
| `GPOS` | `27` | `No` | Geographical Position  |
| `AAAA` | `28` | `Yes` | IPv6 Address  |
| `LOC` | `29` | `Yes` | Location Record  |
| `NXT` | `30` | `No` | RFC 3755  |
| `EID` | `31` | `No` | Endpoint Identifier  |
| `NIMLOC` | `32` | `No` | Nimrod Locator  |
| `SRV` | `33` | `Yes` |  Service Locator  |
| `ATMA` | `34` | `No` |  Asynchronous Transfer Mode Address  |
| `NAPTR` | `35` | `Yes` |  Naming Authority Pointer  |
| `KX` | `36` | `No` |  Key Exchanger  |
| `CERT` | `37` | `No` |  Certificate Record  |
| `A6` | `38` | `No` |  Service Locator  |
| `DNAME` | `39` | `No` |  Non-Terminal DNS Name Redirection  |
| `SINK` | `40` | `No` |  Kitchen Sink  |
| `OPT` | `41` | `Yes` |  Option  |
| `APL` | `42` | `No` |  Address Prefix List  |

## Platforms
- [x] Alpine: 3.10+
- [x] Debian: 9+
- [x] Ubuntu: 16.04+
- [x] Fedora: 29+
- [x] RHEL: 6+
- [x] openSUSE: 15+
- [x] SUSE Enterprise Linux (SLES): 12 SP2+
- [x] macOS: 10.13+
- [x] Windows Client: 7, 8.1, 10 (1607+)
- [x] Windows Server: 2012 R2+

## Chips
- [x] x64 on Windows, macOS, and Linux
- [x] x86 on Windows
- [x] ARM32 on Windows and Linux
- [x] ARM64 on Linux (kernel 4.14+)

## Launch
The Project offically launched within my talk for [Cairo Security Camp](https://cairosecuritycamp.com/sessions/rebuilding-the-domain-name-system/) on September 22nd.

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

## Donations
* **PayPal**: https://www.paypal.me/texnomic
* **Bitcoin**: 13wMqy8yg9yhJAAP2AXu8A2De1ptAYh6s4
* **Ethereum**: 0xfE171b1C5C5584b65ec58a6FA2009f6ECeE812D7

## Supported-By
![JetBrains](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/JetBrains.png "JetBrains")
![Syncfusion](https://raw.githubusercontent.com/Texnomic/SecureDNS/master/docs/Syncfusion.png "Syncfusion")

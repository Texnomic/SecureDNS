namespace Texnomic.FilterLists.Enums
{
    public enum Syntax : int
    {
        /// <summary>
        /// Hosts (localhost) <see cref="https://en.wikipedia.org/wiki/Hosts_(file)"/>
        /// </summary>
        Hosts = 1,

        Domains = 2,

        /// <summary>
        /// Domains <see cref="https://adblockplus.org/filters"/>
        /// </summary>
        AdblockPlus = 3,

        /// <summary>
        /// uBlock Origin Static <see cref="https://github.com/gorhill/uBlock/wiki/Static-filter-syntax"/>
        /// </summary>
        uBlockOriginStatic = 4,

        /// <summary>
        /// AdGuard <see cref="https://kb.adguard.com/en/general/how-to-create-your-own-ad-filters"/>
        /// </summary>
        AdGuard = 6,

        /// <summary>
        /// uMatrix / uBlock Origin Dynamic <see cref="https://github.com/gorhill/uBlock/wiki/Dynamic-filtering:-quick-guide"/>
        /// </summary>
        uBlockOriginDynamic = 7,

        /// <summary>
        /// URLs
        /// </summary>
        URLs = 8,

        /// <summary>
        /// IPs (Singular or singular+range)
        /// </summary>
        IPs = 9,

        /// <summary>
        /// Tracking Protection List (IE) <see cref="https://blogs.msdn.microsoft.com/ie/2010/12/07/ie9-and-privacy-introducing-tracking-protection/"/>
        /// </summary>
        TrackingProtectionList = 10,

        /// <summary>
        /// JavaScript (Non-Safari)
        /// </summary>
        JavaScript = 11,

        /// <summary>
        /// Nano Adblocker Static <see cref="https://github.com/NanoAdblocker/NanoCore/tree/master/notes"/>
        /// </summary>
        NanoAdblockerStatic = 12,

        /// <summary>
        /// MinerBlock <see cref="https://github.com/xd4rker/MinerBlock"/>
        /// </summary>
        MinerBlock = 13,

        /// <summary>
        /// Non-localhost hosts <see cref="https://en.wikipedia.org/wiki/Hosts_(file)"/>
        /// </summary>
        NonHosts = 14,

        /// <summary>
        /// IPs (Range-only)
        /// </summary>
        IPsRange = 15,

        /// <summary>
        /// Domains with wildcards
        /// </summary>
        DomainsWildcards = 16,

        /// <summary>
        /// uBlock Origin scriptlet injection <see cref="https://github.com/gorhill/uBlock/wiki/Static-filter-syntax#scriptlet-injection"/>
        /// </summary>
        uBlockOriginScriptletInjection = 17,

        /// <summary>
        /// Little Snitch subscription-style rules <see cref="https://help.obdev.at/littlesnitch/lsc-rule-group-subscriptions"/>
        /// </summary>
        LittleSnitchSubscriptionStyleRules = 18,

        /// <summary>
        /// Privoxy action file <see cref="https://www.privoxy.org/user-manual/actions-file.html"/>
        /// </summary>
        PrivoxyActionFile = 19,

        /// <summary>
        /// dnsmasq domains list
        /// </summary>
        DnsmasqDomainsList = 20,

        /// <summary>
        /// uBlock Origin !#include-tag compilation <see cref="https://github.com/gorhill/uBlock/wiki/Static-filter-syntax#include-file-name"/>
        /// </summary>
        uBlockOrigin = 21,

        /// <summary>
        /// DNS servers
        /// </summary>
        DNSServers = 22,

        /// <summary>
        /// Unix-format hosts.deny file
        /// </summary>
        UnixHostsDenyFile = 23,

        /// <summary>
        /// Unbound
        /// </summary>
        Unbound = 24,

        /// <summary>
        /// Response Policy Zones (RPZ)
        /// </summary>
        ResponsePolicyZones = 25,

        /// <summary>
        /// BIND
        /// </summary>
        BIND = 26,

        /// <summary>
        /// Windows command line script
        /// </summary>
        WindowsCommandLineScript = 27,

        /// <summary>
        /// Adblocker-syntax domains
        /// </summary>
        AdblockerSyntaxDomains = 28,

        /// <summary>
        /// Socks5
        /// </summary>
        Socks5 = 29,

        /// <summary>
        /// Pi-hole regular expressions <see cref="https://docs.pi-hole.net/ftldns/regex/tutorial/"/>
        /// </summary>
        PiHoleRegularExpressions = 30,
    }
}

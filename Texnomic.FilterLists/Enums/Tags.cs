namespace Texnomic.FilterLists.Enums;

public enum Tags : int
{
    /// <summary>
    /// Blocks cryptomining and/or cryptojacking
    /// </summary>
    Crypto = 1,


    /// <summary>
    /// Blocks advertisements
    /// </summary>
    Ads = 2,


    /// <summary>
    /// Blocks trackers and other privacy-invasive resources
    /// </summary>
    Privacy = 3,


    /// <summary>
    /// Blocks social media scripts, trackers, widgets, comment sections, etc.
    /// </summary>
    Social = 4,


    /// <summary>
    /// Blocks adblock detection scripts
    /// </summary>
    AntiAdblock = 5,


    /// <summary>
    /// Blocks malicious resources
    /// </summary>
    Malware = 6,


    /// <summary>
    /// Blocks phishing and/or scam resources
    /// </summary>
    Phishing = 7,


    /// <summary>
    /// Blocks cookie notices primarily in response to the EU Cookie Law
    /// </summary>
    Cookies = 8,


    /// <summary>
    /// Blocks subjectively annoying resources
    /// </summary>
    Annoyances = 9,


    /// <summary>
    /// Unblocks categorical resources
    /// </summary>
    Whitelist = 10,


    /// <summary>
    /// Blocks adult, NSFW, pornographic, etc. resources
    /// </summary>
    Nsfw = 11,


    /// <summary>
    /// Redirects traffic through proxies to get around firewalls
    /// </summary>
    Proxy = 12,


    /// <summary>
    /// Extends or blocks functionality from search engines
    /// </summary>
    Search = 13,


    /// <summary>
    /// Intended for research only
    /// </summary>
    Research = 14,


    /// <summary>
    /// Blocks specific topics/things
    /// </summary>
    Topical = 15,


    /// <summary>
    /// Removes obstructing or annoying overlays
    /// </summary>
    Overlay = 16,


    /// <summary>
    /// Blocks gambling resources
    /// </summary>
    Gambling = 17,


    /// <summary>
    /// Removes website-embedded fonts
    /// </summary>
    Fonts = 18,


    /// <summary>
    /// Blocks resources from certain companies
    /// </summary>
    AntiCorp = 19,


    /// <summary>
    /// Lists that are of special interest to IT admins
    /// </summary>
    Admin = 20,


    /// <summary>
    /// Lists that remove news stories of subjectively low quality
    /// </summary>
    Clickbait = 21,


    /// <summary>
    /// Blocks religious or superstitious content
    /// </summary>
    Religious = 22,


    /// <summary>
    /// Blocks pages from link-shortening services
    /// </summary>
    Shorteners = 23,


    /// <summary>
    /// Blocks piracy-focusing sites
    /// </summary>
    Piracy = 24,


    /// <summary>
    /// Blocks political content
    /// </summary>
    Politics = 25,


    /// <summary>
    /// Blocks software updates
    /// </summary>
    Updates = 26,


    /// <summary>
    /// Userstyles in adblocker syntax that change the appearance of sites
    /// </summary>
    Userstyle = 27,


    /// <summary>
    /// Blocks requests that ask you to subscribe to newsletters
    /// </summary>
    Newsletters = 28,


    /// <summary>
    /// Prevents How can I help you? prompts from popping up
    /// </summary>
    Helpprompt = 29,


    /// <summary>
    /// Blocks certain cultural content
    /// </summary>
    Cultural = 30,
}
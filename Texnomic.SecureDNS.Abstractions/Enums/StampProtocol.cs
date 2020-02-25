namespace Texnomic.SecureDNS.Abstractions.Enums
{
	/// <summary>
	/// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps"/>
	/// </summary>
	public enum StampProtocol : byte
	{
        DoU = 0,
		DNSCrypt = 1,
		DoH = 2,
		DoT = 3,
		DNSCryptRelay = 129,
        Unknown
	}
}

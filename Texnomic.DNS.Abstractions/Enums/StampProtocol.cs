namespace Texnomic.DNS.Abstractions.Enums
{
	public enum StampProtocol : byte
	{
		Plain = 0,
		DnsCrypt = 1,
		DoH = 2,
		TLS = 3,
		DNSCryptRelay = 129,
		Unknown
	}
}

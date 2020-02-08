namespace Texnomic.DNS.Abstractions.Enums
{
	public enum StampProtocol : byte
	{
		DoU = 0,
		DnsCrypt = 1,
		DoH = 2,
		DoT = 3,
		DNSCryptRelay = 129,
		Unknown
	}
}

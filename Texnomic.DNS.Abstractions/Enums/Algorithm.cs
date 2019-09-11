namespace Texnomic.DNS.Abstractions.Enums
{
    /// <summary>
    /// <see cref="https://docs.infoblox.com/display/NAG8/DNSKEY+Resource+Records"/>
    /// <see cref="https://tools.ietf.org/html/rfc4034#appendix-A.1"/>
    /// </summary>
    public enum Algorithm : byte
    {
        RSA_MD5 = 1,
        DiffieHellman = 2,
        DSA = 3,
        EllipticCurve = 4,
        RSA_SHA1 = 5,
        DSA_SHA1_NSEC3 = 6,
        RSA_SHA1_NSEC3 = 7,
        RSA_SHA256 = 8,
        RSA_SHA512 = 10,
        Indirect = 252,
        PrivateDNS = 253,
        PrivateOID = 254,
        Reserved = 255
    }
}

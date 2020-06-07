using System;

namespace Texnomic.SecureDNS.Abstractions.Enums
{
    /// <summary>
    /// TYPE fields are used in resource records. Note that these types are a subset of QTYPEs.
    /// https://www.iana.org/assignments/dns-parameters/dns-parameters.xhtml#dns-parameters-5
    /// </summary>
    public enum RecordType : ushort
    {
        /// <summary>
        /// Transaction Signatures
        /// </summary>
        SIG0 = 0,

        /// <summary> 
        /// IPv4 Address
        /// </summary>
        A = 1,

        /// <summary> 
        /// Authoritative Name Server
        /// </summary>
        NS = 2,

        /// <summary> 
        /// Mail Destination  
        /// </summary>
        /// <remarks>
        /// Use MX
        /// </remarks>
        [Obsolete]
        MD = 3,

        /// <summary> 
        /// Mail Forwarder  
        /// </summary>
        /// <remarks>
        /// Use MX
        /// </remarks>
        [Obsolete]
        MF = 4,

        /// <summary> 
        /// Canonical Name for An Alias 
        /// </summary>
        CNAME = 5,

        /// <summary> 
        /// Marks The Start of A Zone of Authority
        /// </summary>
        SOA = 6,

        /// <summary> 
        /// Mailbox Domain 
        /// </summary>
        /// <remarks>
        /// EXPERIMENTAL
        /// </remarks>
        [Obsolete]
        MB = 7,

        /// <summary> 
        /// Mail Group Member 
        /// </summary>
        [Obsolete]
        MG = 8,

        /// <summary> 
        /// Mail Rename Domain Name 
        /// </summary>
        [Obsolete]
        MR = 9,

        /// <summary> 
        /// RFC 1035 
        /// </summary>
        NULL = 10,

        /// <summary> 
        /// A Well Known Service Description
        /// </summary>
        WKS = 11,

        /// <summary> Pointer record </summary>
        PTR = 12,

        /// <summary> Not obsoleted. Currently used by Cloudflare in response to queries of the type ANY.[15] </summary>
        HINFO = 13,

        /// <summary>  </summary>
        MINFO = 14,

        /// <summary> Mail exchange record </summary>
        MX = 15,

        /// <summary> Text record </summary>
        TXT = 16,

        /// <summary> Responsible Person </summary>
        RP = 17,

        /// <summary> AFS database record </summary>
        AFSDB = 18,

        /// <summary>  </summary>
        X25 = 19,

        /// <summary>  </summary>
        ISDN = 20,

        /// <summary>  </summary>
        RT = 21,

        /// <summary>  </summary>
        NSAP = 22,

        /// <summary>  </summary>
        NSAP_PTR = 23,

        /// <summary> Signature </summary>
        SIG = 24,

        /// <summary> Key record </summary>
        KEY = 25,

        /// <summary>  </summary>
        PX = 26,

        /// <summary> 
        /// Geographical Position
        /// </summary>
        GPOS = 27,

        /// <summary> 
        /// IPv6 Address 
        /// </summary>
        AAAA = 28,

        /// <summary> Location record </summary>
        LOC = 29,

        /// <summary> RFC 3755 </summary>
        NXT = 30,

        /// <summary>
        /// Endpoint Identifier
        /// </summary>
        [Obsolete]
        EID = 31,

        /// <summary>
        /// Nimrod Locator
        /// </summary>
        NIMLOC = 32,

        /// <summary> Service locator </summary>
        SRV = 33,

        /// <summary>  </summary>
        ATMA = 34,

        /// <summary> Naming Authority Pointer </summary>
        NAPTR = 35,

        /// <summary> Key Exchanger record </summary>
        KX = 36,

        /// <summary> Certificate record </summary>
        CERT = 37,

        /// <summary> RFC 6563 </summary>
        A6 = 38,

        /// <summary>  </summary>
        DNAME = 39,

        /// <summary>  </summary>
        SINK = 40,

        /// <summary> Option </summary>
        OPT = 41,

        /// <summary> Address Prefix List </summary>
        APL = 42,

        /// <summary> Delegation signer </summary>
        DS = 43,

        /// <summary> SSH Public Key Fingerprint </summary>
        SSHFP = 44,

        /// <summary> IPsec Key </summary>
        IPSECKEY = 45,

        /// <summary> DNSSEC signature </summary>
        RRSIG = 46,

        /// <summary> Next Secure record </summary>
        NSEC = 47,

        /// <summary> DNS Key record </summary>
        DNSKEY = 48,

        /// <summary> DHCP identifier </summary>
        DHCID = 49,

        /// <summary> Next Secure record version 3 </summary>
        NSEC3 = 50,

        /// <summary> NSEC3 parameters </summary>
        NSEC3PARAM = 51,

        /// <summary> TLSA certificate association </summary>
        TLSA = 52,

        /// <summary> S/MIME cert association[10] </summary>
        SMIMEA = 53,

        /// <summary> Host Identity Protocol </summary>
        HIP = 55,

        /// <summary>  </summary>
        NINFO = 56,

        /// <summary>  </summary>
        RKEY = 57,

        /// <summary> Child DS </summary>
        CDS = 59,

        /// <summary>  </summary>
        CDNSKEY = 60,

        /// <summary> OpenPGP public key record </summary>
        OPENPGPKEY = 61,

        /// <summary> RFC 7208 </summary>
        SPF = 99,

        /// <summary>  </summary>
        UINFO = 100,

        /// <summary>  </summary>
        UID = 101,

        /// <summary>  </summary>
        GID = 102,

        /// <summary>  </summary>
        UNSPEC = 103,

        /// <summary> Transaction Key record </summary>
        TKEY = 249,

        /// <summary> Transaction Signature </summary>
        TSIG = 250,

        /// <summary> Incremental Zone Transfer </summary>
        IXFR = 251,

        /// <summary> Authoritative Zone Transfer </summary>
        AXFR = 252,

        /// <summary>  </summary>
        MAILB = 253,

        /// <summary>  </summary>
        MAILA = 254,

        /// <summary> All cached records </summary>
        Any = 255,

        /// <summary> Uniform Resource Identifier </summary>
        URI = 256,

        /// <summary> 
        /// Certification Authority Authorization
        /// </summary>
        CAA = 257,

        /// <summary>
        /// Application Visibility and Control
        /// </summary>
        AVC = 258,

        /// <summary>
        /// Digital Object Architecture
        /// </summary>
        DOA = 259,

        /// <summary>
        /// Automatic Multicast Tunneling Relay
        /// </summary>
        AMTRELAY = 260,

        /// <summary> 
        /// DNSSEC Trust Authorities
        /// </summary>
        TA = 32768,

        /// <summary>
        /// DNSSEC Lookaside Validation
        /// </summary>
        DLV = 32769,
    }
}

namespace Texnomic.SecureDNS.Abstractions.Enums;

public enum ResponseCode : byte
{
    /// <summary>
    /// No Error Condition.
    /// </summary>
    NoError = 0,

    /// <summary>
    /// The name server was unable to interpret the query.
    /// </summary>
    FormatError = 1,

    /// <summary>
    /// The name server was unable to process this query due to a problem with the name server.
    /// </summary>
    ServerFailure = 2,

    /// <summary>
    /// Non-Existent Domain.
    /// </summary>
    NonExistentDoman = 3,

    /// <summary>
    /// The name server does not support the requested kind of query.
    /// </summary>
    NotImplemented = 4,

    /// <summary>
    /// The name server refuses to perform the specified operation for policy reasons.
    /// </summary>
    Refused = 5,

    /// <summary> 
    /// Name Exists when it should not.
    /// </summary>
    YXDomain = 6,

    /// <summary> 
    /// RR Set Exists when it should not.
    /// </summary>
    YXRRSet = 7,

    /// <summary> 
    /// RR Set that should exist does not.
    /// </summary>
    NXRRSet = 8,

    /// <summary> 
    /// Not Authorized.
    /// </summary>
    NotAuthorized = 9,

    /// <summary> 
    /// Name not contained in zone.
    /// </summary>
    NotZone = 10,

    /// <summary> 
    /// DSO-TYPE Not Implemented
    /// </summary>
    DSOTYPENI = 11,

    /// <summary> 
    /// Bad OPT Version Or TSIG Signature Failure.
    /// </summary>
    BadVersionOrSignature = 16,

    /// <summary> 
    /// Key not recognized
    /// </summary>
    BADKEY = 17,

    /// <summary> 
    /// Signature out of time window
    /// </summary>
    BADTIME = 18,

    /// <summary> 
    /// Bad TKEY Mode
    /// </summary>
    BADMODE = 19,

    /// <summary> 
    /// Duplicate key name
    /// </summary>
    BADNAME = 20,

    /// <summary> 
    /// Algorithm not supported
    /// </summary>
    BADALG = 21,

    /// <summary> 
    /// Bad Truncation
    /// </summary>
    BADTRUNC = 22,

    /// <summary> 
    /// Bad/missing Server Cookie
    /// </summary>
    BADCOOKIE = 23,
}
namespace Texnomic.SecureDNS.Abstractions.Enums;

public enum RecordClass : ushort
{
    /// <summary>
    /// Reserved
    /// </summary>
    Reserved = 0,

    /// <summary>
    /// Internet
    /// </summary>
    Internet = 1,

    /// <summary>
    /// CSNET
    /// </summary>
    CSNET = 2,


    /// <summary>
    /// CHAOS
    /// </summary>
    CHAOS = 3,

    /// <summary>
    /// Hesiod
    /// </summary>
    Hesiod = 4,

    /// <summary>
    /// None
    /// </summary>
    None = 254,

    /// <summary>
    /// Any
    /// </summary>
    Any = 255,
}
using System;

namespace Texnomic.SecureDNS.Abstractions.Enums;

public enum OperationCode : byte
{
    Query = 0,

    /// <summary>
    /// Inverse Query
    /// </summary>
    [Obsolete]
    IQuery = 1,

    Status = 2,
    Reserved = 3,
    Notify = 4,
    Update = 5,

    /// <summary>
    /// DNS Stateful Operations (DSO)
    /// </summary>
    DSO = 6
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Texnomic.Socks5.Enum
{
    internal enum Reply: byte
    {
        Succeeded = 0,
        Failure = 1,
        NotAllowed = 2,
        NetworkUnreachable = 3,
        HostUnreachable = 4,
        ConnectionRefused = 5,
        TTLExpired = 6,
        CommandNotSupported = 8
    }
}

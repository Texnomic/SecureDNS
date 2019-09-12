using System;
using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface ITimeToLive
    {
        [Ignore]
        TimeSpan Value { get; set; }
    }
}

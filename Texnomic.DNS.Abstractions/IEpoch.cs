using System;
using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface IEpoch
    {
        [Ignore] 
        DateTime Value { get; set; }
    }
}

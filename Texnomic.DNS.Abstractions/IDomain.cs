using System.Collections.Generic;
using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface IDomain
    {
        [Ignore]
        List<ILabel> Labels { get; }

        [Ignore]
        string Name { get; }
    }
}

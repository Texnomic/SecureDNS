using System.Collections.Generic;

namespace Texnomic.SecureDNS.Abstractions
{
    public interface IDomain
    {
        IEnumerable<string> Labels { get; }

        string Name { get; }
    }
}

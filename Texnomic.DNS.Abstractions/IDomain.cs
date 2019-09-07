using System.Collections.Generic;

namespace Texnomic.DNS.Abstractions
{
    public interface IDomain
    {
        List<ILabel> Labels { get; }

        string Name { get; }

        //IDomain FromString(string Domain);
    }
}

using Destructurama.Attributed;
using System.Collections.Generic;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.DataTypes;

[LogAsScalar(true)]
public class Domain : IDomain
{
    public IEnumerable<string> Labels { get; set; }

    public Domain()
    {
        Labels = new List<string>();
    }

    public string Name => ToString();

    public static implicit operator string(Domain Domain)
    {
        return Domain?.ToString();
    }

    public static implicit operator Domain(string FQDN)
    {
        return FromString(FQDN);
    }

    public override string ToString()
    {
        return string.Join('.', Labels);
    }

    public static Domain FromString(string FQDN)
    {
        return new Domain()
        {
            Labels = FQDN.Split('.')
        };
    }
}
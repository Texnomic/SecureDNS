using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.DataTypes;

[LogAsScalar(true)]
public class Base64String : IBase64String
{
    public byte[] Bytes { get; set; }

    public string Base64
    {
        get => Convert.ToBase64String(Bytes);
        set => Bytes = Convert.FromBase64String(value);
    }

    public static implicit operator string(Base64String Base64String)
    {
        return Base64String.ToString();
    }

    public override string ToString()
    {
        return Base64;
    }
}
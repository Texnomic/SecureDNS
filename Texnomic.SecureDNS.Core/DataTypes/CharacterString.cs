using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.DataTypes;

/// <summary>
/// Character-String is a single length octet followed by that number of characters.
/// <see cref="https://tools.ietf.org/html/rfc1035#section-3.3"/>
/// </summary>
[LogAsScalar(true)]
public class CharacterString : ICharacterString
{
    public byte Length { get; set; }

    public string Value { get; set; }

    public static implicit operator string(CharacterString CharacterString)
    {
        return CharacterString.Value;
    }

    public static implicit operator CharacterString(string String)
    {
        return new CharacterString()
        {
            Length = (byte)String.Length,
            Value = String,
        };
    }

    public override string ToString() => Value;
}
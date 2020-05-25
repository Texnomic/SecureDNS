using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Core.DataTypes;

namespace Texnomic.SecureDNS.Core.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                   TXT-DATA                    /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Text Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.14">(TXT)</see>
    /// </summary>
    [LogAsScalar(true)]
    public class TXT : IRecord
    {
        public ICharacterString Text { get; set; }

        public ICertificate Certificate { get; set; }

        public static implicit operator string(TXT TXT)
        {
            return TXT.ToString();
        }

        public static implicit operator TXT(string Text)
        {
            return new TXT()
            {
                Text = new CharacterString()
                {
                    Length = (byte)Text.Length,
                    Value = Text
                }
            };
        }

        public override string ToString()
        {
            return Certificate != null ? Certificate.ToString() : Text.ToString();
        }
    }
}

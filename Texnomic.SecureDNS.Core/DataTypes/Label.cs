using System.Text;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    public struct Label : ILabel
    {
        public byte[] Bytes { get; set; }

        public static implicit operator string(Label Label)
        {
            return Label.ToString();
        }

        public static implicit operator Label(string Label)
        {
            return FromString(Label);
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(Bytes);
        }

        public static Label FromString(string Label)
        {
            return new Label()
            {
                Bytes = Encoding.ASCII.GetBytes(Label)
            };
        }
    }
}

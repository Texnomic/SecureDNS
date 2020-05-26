using Destructurama.Attributed;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#common-definitions"/>
    /// </summary>
    [LogAsScalar(true)]
    public class PrefixedString
    {
        public byte Length { get; set; }

        public string Value { get; set; }

        public static implicit operator string(PrefixedString PrefixedString)
        {
            return PrefixedString?.Value;
        }

        public override string ToString() => Value;
    }
}

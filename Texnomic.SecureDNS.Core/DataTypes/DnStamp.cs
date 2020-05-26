using System;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    public class DnStamp : IDnStamp
    {
        public StampProtocol Protocol { get; set; }

        public IStamp Value { get; set; }

        public static DnStamp FromString(string Stamp)
        {
            throw new NotImplementedException();
        }

        public static async Task<IDnStamp> FromStringAsync(string Stamp)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public async Task<string> ToStringAsync()
        {
            throw new NotImplementedException();
        }

        private static string Encode(byte[] Stamp)
        {
            return Convert.ToBase64String(Stamp)
                .Replace("=", "")
                .Replace("/", "_")
                .Replace("+", "-");
        }

        private static string ToBase64(string Stamp)
        {
            return Stamp
                .PadRight(Stamp.Length + (4 - Stamp.Length % 4) % 4, '=')
                .Replace("_", "/")
                .Replace("-", "+");
        }

        private static byte[] Decode(string Stamp)
        {
            return Convert.FromBase64String(ToBase64(Stamp));
        }
    }


}

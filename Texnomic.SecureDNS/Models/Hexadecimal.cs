using System;
using System.Linq;

namespace Texnomic.SecureDNS.Models
{
    public class Hexadecimal
    {
        public byte[] Raw { get; private set; }

        public Hexadecimal(byte[] Raw)
        {
            this.Raw = Raw;
        }

        public static Hexadecimal Parse(string Hex)
        {
            var Bytes = Enumerable.Range(0, Hex.Length)
                                  .Where(x => x % 2 == 0)
                                  .Select(x => Convert.ToByte(Hex.Substring(x, 2), 16))
                                  .ToArray();

            return new Hexadecimal(Bytes);
        }

        public string ToHexadecimalString()
        {
            return BitConverter.ToString(Raw);
        }

        public override string ToString()
        {
            return ToHexadecimalString();
        }
    }
}

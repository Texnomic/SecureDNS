using System;

namespace Texnomic.SecureDNS.Serialization.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] TrimEnd(this byte[] Raw, byte Byte = 0)
        {
            for (var I = Raw.Length - 1; I > -1; I--)
            {
                if (Raw[I] != Byte) return Raw[..++I];
            }

            return Raw;
        }

        public static byte[] Concat(this byte[] FirstArray, byte[] SecondArray)
        {
            var ConcatArray = new byte[FirstArray.Length + SecondArray.Length];

            Array.Copy(FirstArray, ConcatArray, FirstArray.Length);

            Array.Copy(SecondArray, 0, ConcatArray, FirstArray.Length - 1, SecondArray.Length);

            return ConcatArray;
        }
    }

}

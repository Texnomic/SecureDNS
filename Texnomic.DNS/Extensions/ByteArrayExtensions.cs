using System;

namespace Texnomic.DNS.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] TrimEnd(this byte[] Data, byte Byte = 0)
        {
            for (var I = Data.Length - 1; I > -1; I--)
            {
                if (Data[I] != Byte) return Data[..++I];
            }

            return Data;
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

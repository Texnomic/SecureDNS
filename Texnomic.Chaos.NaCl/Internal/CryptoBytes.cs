using System;
using System.Runtime.CompilerServices;

namespace Texnomic.Chaos.NaCl.Internal
{
    public static class CryptoBytes
    {
        public static bool ConstantTimeEquals(byte[] x, int Offset, byte[] Y, int YOffset, int Length)
        {
            if (x is null)
                throw new ArgumentNullException(nameof(x));

            if (Offset < 0)
                throw new ArgumentOutOfRangeException("Offset", "xOffset < 0");
            if (Y == null)
                throw new ArgumentNullException(nameof(Y));

            if (YOffset < 0)
                throw new ArgumentOutOfRangeException("YOffset", "yOffset < 0");

            if (Length < 0)
                throw new ArgumentOutOfRangeException("Length", "length < 0");

            if (x.Length - Offset < Length)
                throw new ArgumentException("xOffset + length > x.Length");

            if (Y.Length - YOffset < Length)
                throw new ArgumentException("yOffset + length > y.Length");

            return InternalConstantTimeEquals(x, Offset, Y, YOffset, Length) != 0;
        }

        private static uint InternalConstantTimeEquals(ReadOnlySpan<byte> x, int Offset, ReadOnlySpan<byte> Y, int YOffset, int Length)
        {
            var differentbits = 0;
            for (var i = 0; i < Length; i++)
                differentbits |= x[Offset + i] ^ Y[YOffset + i];

            return (1 & (unchecked((uint)differentbits - 1) >> 8));
        }

        public static void Wipe(byte[] Data)
        {
            if (Data is null)
                throw new ArgumentNullException(nameof(Data));

            InternalWipe(Data, 0, Data.Length);
        }

        // Secure wiping is hard
        // * the GC can move around and copy memory
        //   Perhaps this can be avoided by using unmanaged memory or by fixing the position of the array in memory
        // * Swap files and error dumps can contain secret information
        //   It seems possible to lock memory in RAM, no idea about error dumps
        // * Compiler could optimize out the wiping if it knows that data won't be read back
        //   I hope this is enough, suppressing inlining
        //   but perhaps `RtlSecureZeroMemory` is needed
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void InternalWipe(byte[] Data, int Offset, int Count) => Array.Clear(Data, Offset, Count);


    }
}

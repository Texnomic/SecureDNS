using System.Runtime.CompilerServices;

namespace Texnomic.Chaos.NaCl.Internal
{
    internal static class BitUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(uint Value, int Offset)
        {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
            return (Value << Offset) | (Value >> (32 - Offset));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong Value, int Offset) // Taken help from: https://stackoverflow.com/a/48580489/5592276
        {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
            return (Value << Offset) | (Value >> (64 - Offset));
#endif
        }
    }
}

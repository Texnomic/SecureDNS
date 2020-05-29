namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class ScalarOperations
    {
        public static void sc_clamp(byte[] S, int Offset)
        {
            S[Offset + 0] &= 248;
            S[Offset + 31] &= 127;
            S[Offset + 31] |= 64;
        }
    }
}
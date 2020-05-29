using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class Ed25519Operations
    {
        public static void crypto_sign_keypair(byte[] Pk, int Pkoffset, byte[] Sk, int Skoffset, byte[] Seed, int Seedoffset)
        {
            GroupElementP3 A;
            int i;

            Array.Copy(Seed, Seedoffset, Sk, Skoffset, 32);
            var h = Sha512.Hash(Sk, Skoffset, 32);//ToDo: Remove alloc
            ScalarOperations.sc_clamp(h, 0);

            GroupOperations.ge_scalarmult_base(out A, h, 0);
            GroupOperations.ge_p3_tobytes(Pk, Pkoffset, ref A);

            for (i = 0; i < 32; ++i) Sk[Skoffset + 32 + i] = Pk[Pkoffset + i];
            CryptoBytes.Wipe(h);
        }
    }
}

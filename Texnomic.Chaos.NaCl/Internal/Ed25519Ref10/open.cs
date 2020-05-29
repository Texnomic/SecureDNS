using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class Ed25519Operations
    {
        // Original crypto_sign_open, for reference only
        /*public static int crypto_sign_open(
          byte[] m, out int mlen,
          byte[] sm, int smlen,
          byte[] pk)
        {
            byte[] h = new byte[64];
            byte[] checkr = new byte[32];
            GroupElementP3 A;
            GroupElementP2 R;
            int i;

            mlen = -1;
            if (smlen < 64) return -1;
            if ((sm[63] & 224) != 0) return -1;
            if (GroupOperations.ge_frombytes_negate_vartime(out A, pk, 0) != 0) return -1;

            for (i = 0; i < smlen; ++i) m[i] = sm[i];
            for (i = 0; i < 32; ++i) m[32 + i] = pk[i];
            Sha512BclWrapper.crypto_hash_sha512(h, m, 0, smlen);
            ScalarOperations.sc_reduce(h);

            var sm32 = new byte[32];
            Array.Copy(sm, 32, sm32, 0, 32);
            GroupOperations.ge_double_scalarmult_vartime(out R, h, ref A, sm32);
            GroupOperations.ge_tobytes(checkr, 0, ref R);
            if (Helpers.crypto_verify_32(checkr, sm) != 0)
            {
                for (i = 0; i < smlen; ++i)
                    m[i] = 0;
                return -1;
            }

            for (i = 0; i < smlen - 64; ++i)
                m[i] = sm[64 + i];
            for (i = smlen - 64; i < smlen; ++i)
                m[i] = 0;
            mlen = smlen - 64;
            return 0;
        }*/

        public static bool crypto_sign_verify(
            byte[] Sig, int Sigoffset,
            byte[] M, int Moffset, int Mlen,
            byte[] Pk, int Pkoffset)
        {
            byte[] h;
            var checkr = new byte[32];
            GroupElementP3 A;
            GroupElementP2 R;

            if ((Sig[Sigoffset + 63] & 224) != 0) return false;
            if (GroupOperations.ge_frombytes_negate_vartime(out A, Pk, Pkoffset) != 0)
                return false;

            var hasher = new Sha512();
            hasher.Update(Sig, Sigoffset, 32);
            hasher.Update(Pk, Pkoffset, 32);
            hasher.Update(M, Moffset, Mlen);
            h = hasher.Finish();

            ScalarOperations.sc_reduce(h);

            var sm32 = new byte[32];//todo: remove allocation
            Array.Copy(Sig, Sigoffset + 32, sm32, 0, 32);
            GroupOperations.ge_double_scalarmult_vartime(out R, h, ref A, sm32);
            GroupOperations.ge_tobytes(checkr, 0, ref R);
            var result = CryptoBytes.ConstantTimeEquals(checkr, 0, Sig, Sigoffset, 32);
            CryptoBytes.Wipe(h);
            CryptoBytes.Wipe(checkr);
            return result;
        }
    }
}
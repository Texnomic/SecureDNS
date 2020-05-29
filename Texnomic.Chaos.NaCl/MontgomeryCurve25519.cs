using System;
using Texnomic.Chaos.NaCl.Internal.Ed25519Ref10;
using Texnomic.Chaos.NaCl.Internal.Salsa;

namespace Texnomic.Chaos.NaCl
{
    // This class is mainly for compatibility with NaCl's Curve25519 implementation
    // If you don't need that compatibility, use Ed25519.KeyExchange
    public static class MontgomeryCurve25519
    {
        public static readonly int PublicKeySizeInBytes = 32;
        public static readonly int PrivateKeySizeInBytes = 32;
        public static readonly int SharedKeySizeInBytes = 32;

        public static byte[] GetPublicKey(byte[] PrivateKey)
        {
            if (PrivateKey == null)
                throw new ArgumentNullException(nameof(PrivateKey));
            if (PrivateKey.Length != PrivateKeySizeInBytes)
                throw new ArgumentException("privateKey.Length must be 32");
            var publicKey = new byte[32];
            GetPublicKey(new ArraySegment<byte>(publicKey), new ArraySegment<byte>(PrivateKey));
            return publicKey;
        }

        public static void GetPublicKey(ArraySegment<byte> PublicKey, ArraySegment<byte> PrivateKey)
        {
            if (PublicKey.Array == null)
                throw new ArgumentNullException("publicKey.Array");
            if (PrivateKey.Array == null)
                throw new ArgumentNullException("privateKey.Array");
            if (PublicKey.Count != PublicKeySizeInBytes)
                throw new ArgumentException("privateKey.Count must be 32");
            if (PrivateKey.Count != PrivateKeySizeInBytes)
                throw new ArgumentException("privateKey.Count must be 32");

            // hack: abusing publicKey as temporary storage
            // todo: remove hack
            for (var i = 0; i < 32; i++)
            {
                PublicKey.Array[PublicKey.Offset + i] = PrivateKey.Array[PrivateKey.Offset + i];
            }
            ScalarOperations.sc_clamp(PublicKey.Array, PublicKey.Offset);

            GroupElementP3 A;
            GroupOperations.ge_scalarmult_base(out A, PublicKey.Array, PublicKey.Offset);
            FieldElement PublicKeyFe;
            EdwardsToMontgomeryX(out PublicKeyFe, ref A.Y, ref A.Z);
            FieldOperations.fe_tobytes(PublicKey.Array, PublicKey.Offset, ref PublicKeyFe);
        }

        private static readonly byte[] Zero16 = new byte[16];

        // hashes like the NaCl paper says instead i.e. HSalsa(x,0)
        internal static void KeyExchangeOutputHashNaCl(byte[] SharedKey, int Offset)
        {
            Salsa20.HSalsa20(SharedKey, Offset, SharedKey, Offset, Zero16, 0);
        }

        public static byte[] KeyExchange(byte[] PublicKey, byte[] PrivateKey)
        {
            var sharedKey = new byte[SharedKeySizeInBytes];
            KeyExchange(new ArraySegment<byte>(sharedKey), new ArraySegment<byte>(PublicKey), new ArraySegment<byte>(PrivateKey));
            return sharedKey;
        }

        public static void KeyExchange(ArraySegment<byte> SharedKey, ArraySegment<byte> PublicKey, ArraySegment<byte> PrivateKey)
        {
            if (SharedKey.Array == null)
                throw new ArgumentNullException("sharedKey.Array");
            if (PublicKey.Array == null)
                throw new ArgumentNullException("publicKey.Array");
            if (PrivateKey.Array == null)
                throw new ArgumentNullException(nameof(PrivateKey));
            if (SharedKey.Count != 32)
                throw new ArgumentException("sharedKey.Count != 32");
            if (PublicKey.Count != 32)
                throw new ArgumentException("publicKey.Count != 32");
            if (PrivateKey.Count != 32)
                throw new ArgumentException("privateKey.Count != 32");
            MontgomeryOperations.Scalarmult(SharedKey.Array, SharedKey.Offset, PrivateKey.Array, PrivateKey.Offset, PublicKey.Array, PublicKey.Offset);
            KeyExchangeOutputHashNaCl(SharedKey.Array, SharedKey.Offset);
        }

        internal static void EdwardsToMontgomeryX(out FieldElement MontgomeryX, ref FieldElement EdwardsY, ref FieldElement EdwardsZ)
        {
            FieldElement tempX, tempZ;
            FieldOperations.fe_add(out tempX, ref EdwardsZ, ref EdwardsY);
            FieldOperations.fe_sub(out tempZ, ref EdwardsZ, ref EdwardsY);
            FieldOperations.fe_invert(out tempZ, ref tempZ);
            FieldOperations.fe_mul(out MontgomeryX, ref tempX, ref tempZ);
        }
    }
}

using System;
using Texnomic.Chaos.NaCl.Internal;
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

        static readonly byte[] BasePoint = new byte[32]
		{
			9, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0
		};

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

        // hashes like the Curve25519 paper says
        internal static void KeyExchangeOutputHashCurve25519Paper(byte[] SharedKey, int Offset)
        {
            //c = Curve25519output
            const UInt32 C0 = 'C' | 'u' << 8 | 'r' << 16 | (UInt32)'v' << 24;
            const UInt32 C1 = 'e' | '2' << 8 | '5' << 16 | (UInt32)'5' << 24;
            const UInt32 C2 = '1' | '9' << 8 | 'o' << 16 | (UInt32)'u' << 24;
            const UInt32 C3 = 't' | 'p' << 8 | 'u' << 16 | (UInt32)'t' << 24;

            Array16<UInt32> salsaState;
            salsaState.x0 = C0;
            salsaState.x1 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 0);
            salsaState.x2 = 0;
            salsaState.x3 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 4);
            salsaState.x4 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 8);
            salsaState.x5 = C1;
            salsaState.x6 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 12);
            salsaState.x7 = 0;
            salsaState.x8 = 0;
            salsaState.x9 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 16);
            salsaState.x10 = C2;
            salsaState.x11 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 20);
            salsaState.x12 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 24);
            salsaState.x13 = 0;
            salsaState.x14 = ByteIntegerConverter.LoadLittleEndian32(SharedKey, Offset + 28);
            salsaState.x15 = C3;
            SalsaCore.Salsa(out salsaState, ref salsaState, 20);

            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 0, salsaState.x0);
            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 4, salsaState.x1);
            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 8, salsaState.x2);
            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 12, salsaState.x3);
            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 16, salsaState.x4);
            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 20, salsaState.x5);
            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 24, salsaState.x6);
            ByteIntegerConverter.StoreLittleEndian32(SharedKey, Offset + 28, salsaState.x7);
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

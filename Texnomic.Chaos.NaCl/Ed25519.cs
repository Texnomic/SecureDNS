using System;
using Texnomic.Chaos.NaCl.Internal.Ed25519Ref10;

namespace Texnomic.Chaos.NaCl
{
    public static class Ed25519
    {
        public static readonly int PublicKeySizeInBytes = 32;
        public static readonly int SignatureSizeInBytes = 64;
        public static readonly int ExpandedPrivateKeySizeInBytes = 32 * 2;
        public static readonly int PrivateKeySeedSizeInBytes = 32;
        public static readonly int SharedKeySizeInBytes = 32;

        public static bool Verify(ArraySegment<byte> Signature, ArraySegment<byte> Message, ArraySegment<byte> PublicKey)
        {
            if (Signature.Count != SignatureSizeInBytes)
                throw new ArgumentException(string.Format("Signature size must be {0}", SignatureSizeInBytes), "signature.Count");
            if (PublicKey.Count != PublicKeySizeInBytes)
                throw new ArgumentException(string.Format("Public key size must be {0}", PublicKeySizeInBytes), "publicKey.Count");
            return Ed25519Operations.crypto_sign_verify(Signature.Array, Signature.Offset, Message.Array, Message.Offset, Message.Count, PublicKey.Array, PublicKey.Offset);
        }

        public static bool Verify(byte[] Signature, byte[] Message, byte[] PublicKey)
        {
            if (Signature == null)
                throw new ArgumentNullException(nameof(Signature));
            if (Message == null)
                throw new ArgumentNullException(nameof(Message));
            if (PublicKey == null)
                throw new ArgumentNullException(nameof(PublicKey));
            if (Signature.Length != SignatureSizeInBytes)
                throw new ArgumentException(string.Format("Signature size must be {0}", SignatureSizeInBytes), "signature.Length");
            if (PublicKey.Length != PublicKeySizeInBytes)
                throw new ArgumentException(string.Format("Public key size must be {0}", PublicKeySizeInBytes), "publicKey.Length");
            return Ed25519Operations.crypto_sign_verify(Signature, 0, Message, 0, Message.Length, PublicKey, 0);
        }

        public static void Sign(ArraySegment<byte> Signature, ArraySegment<byte> Message, ArraySegment<byte> ExpandedPrivateKey)
        {
            if (Signature.Array == null)
                throw new ArgumentNullException("signature.Array");
            if (Signature.Count != SignatureSizeInBytes)
                throw new ArgumentException("signature.Count");
            if (ExpandedPrivateKey.Array == null)
                throw new ArgumentNullException("expandedPrivateKey.Array");
            if (ExpandedPrivateKey.Count != ExpandedPrivateKeySizeInBytes)
                throw new ArgumentException("expandedPrivateKey.Count");
            if (Message.Array == null)
                throw new ArgumentNullException("message.Array");
            Ed25519Operations.crypto_sign2(Signature.Array, Signature.Offset, Message.Array, Message.Offset, Message.Count, ExpandedPrivateKey.Array, ExpandedPrivateKey.Offset);
        }

        public static byte[] Sign(byte[] Message, byte[] ExpandedPrivateKey)
        {
            var signature = new byte[SignatureSizeInBytes];
            Sign(new ArraySegment<byte>(signature), new ArraySegment<byte>(Message), new ArraySegment<byte>(ExpandedPrivateKey));
            return signature;
        }

        public static byte[] PublicKeyFromSeed(byte[] PrivateKeySeed)
        {
            byte[] privateKey;
            byte[] publicKey;
            KeyPairFromSeed(out publicKey, out privateKey, PrivateKeySeed);
            CryptoBytes.Wipe(privateKey);
            return publicKey;
        }

        public static byte[] ExpandedPrivateKeyFromSeed(byte[] PrivateKeySeed)
        {
            byte[] privateKey;
            byte[] publicKey;
            KeyPairFromSeed(out publicKey, out privateKey, PrivateKeySeed);
            CryptoBytes.Wipe(publicKey);
            return privateKey;
        }

        public static void KeyPairFromSeed(out byte[] PublicKey, out byte[] ExpandedPrivateKey, byte[] PrivateKeySeed)
        {
            if (PrivateKeySeed == null)
                throw new ArgumentNullException(nameof(PrivateKeySeed));
            if (PrivateKeySeed.Length != PrivateKeySeedSizeInBytes)
                throw new ArgumentException("privateKeySeed");
            var pk = new byte[PublicKeySizeInBytes];
            var sk = new byte[ExpandedPrivateKeySizeInBytes];
            Ed25519Operations.crypto_sign_keypair(pk, 0, sk, 0, PrivateKeySeed, 0);
            PublicKey = pk;
            ExpandedPrivateKey = sk;
        }

        public static void KeyPairFromSeed(ArraySegment<byte> PublicKey, ArraySegment<byte> ExpandedPrivateKey, ArraySegment<byte> PrivateKeySeed)
        {
            if (PublicKey.Array == null)
                throw new ArgumentNullException("publicKey.Array");
            if (ExpandedPrivateKey.Array == null)
                throw new ArgumentNullException("expandedPrivateKey.Array");
            if (PrivateKeySeed.Array == null)
                throw new ArgumentNullException("privateKeySeed.Array");
            if (PublicKey.Count != PublicKeySizeInBytes)
                throw new ArgumentException("publicKey.Count");
            if (ExpandedPrivateKey.Count != ExpandedPrivateKeySizeInBytes)
                throw new ArgumentException("expandedPrivateKey.Count");
            if (PrivateKeySeed.Count != PrivateKeySeedSizeInBytes)
                throw new ArgumentException("privateKeySeed.Count");
            Ed25519Operations.crypto_sign_keypair(
                PublicKey.Array, PublicKey.Offset,
                ExpandedPrivateKey.Array, ExpandedPrivateKey.Offset,
                PrivateKeySeed.Array, PrivateKeySeed.Offset);
        }

        [Obsolete("Needs more testing")]
        public static byte[] KeyExchange(byte[] PublicKey, byte[] PrivateKey)
        {
            var sharedKey = new byte[SharedKeySizeInBytes];
            KeyExchange(new ArraySegment<byte>(sharedKey), new ArraySegment<byte>(PublicKey), new ArraySegment<byte>(PrivateKey));
            return sharedKey;
        }

        [Obsolete("Needs more testing")]
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
            if (PrivateKey.Count != 64)
                throw new ArgumentException("privateKey.Count != 64");

            FieldElement montgomeryX, edwardsY, edwardsZ, sharedMontgomeryX;
            FieldOperations.fe_frombytes(out edwardsY, PublicKey.Array, PublicKey.Offset);
            FieldOperations.fe_1(out edwardsZ);
            MontgomeryCurve25519.EdwardsToMontgomeryX(out montgomeryX, ref edwardsY, ref edwardsZ);
            var h = Sha512.Hash(PrivateKey.Array, PrivateKey.Offset, 32);//ToDo: Remove alloc
            ScalarOperations.sc_clamp(h, 0);
            MontgomeryOperations.Scalarmult(out sharedMontgomeryX, h, 0, ref montgomeryX);
            CryptoBytes.Wipe(h);
            FieldOperations.fe_tobytes(SharedKey.Array, SharedKey.Offset, ref sharedMontgomeryX);
            MontgomeryCurve25519.KeyExchangeOutputHashNaCl(SharedKey.Array, SharedKey.Offset);
        }
    }
}
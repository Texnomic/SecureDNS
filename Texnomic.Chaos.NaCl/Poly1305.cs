using System;
using Texnomic.Chaos.NaCl.Internal;

namespace Texnomic.Chaos.NaCl
{
    internal sealed class Poly1305 : OneTimeAuth
    {
        public override int KeySizeInBytes => 32;

        public override int SignatureSizeInBytes => 16;

        [Obsolete("Needs more testing")]
        public override byte[] Sign(byte[] Message, byte[] Key)
        {
            if (Message == null)
                throw new ArgumentNullException(nameof(Message));
            if (Key == null)
                throw new ArgumentNullException(nameof(Key));
            if (Key.Length != 32)
                throw new ArgumentException("Invalid key size", nameof(Key));

            var result = new byte[16];
            Array8<UInt32> internalKey;
            ByteIntegerConverter.Array8LoadLittleEndian32(out internalKey, Key, 0);
            Poly1305Donna.poly1305_auth(result, 0, Message, 0, Message.Length, ref internalKey);
            return result;
        }

        [Obsolete("Needs more testing")]
        public override void Sign(ArraySegment<byte> Signature, ArraySegment<byte> Message, ArraySegment<byte> Key)
        {
            if (Signature.Array == null)
                throw new ArgumentNullException(nameof(Key));
            if (Message.Array == null)
                throw new ArgumentNullException(nameof(Message));
            if (Key.Array == null)
                throw new ArgumentNullException(nameof(Signature));
            if (Key.Count != 32)
                throw new ArgumentException("Invalid key size", nameof(Key));
            if (Signature.Count != 16)
                throw new ArgumentException("Invalid signature size", nameof(Signature));

            Array8<UInt32> internalKey;
            ByteIntegerConverter.Array8LoadLittleEndian32(out internalKey, Key.Array, Key.Offset);
            Poly1305Donna.poly1305_auth(Signature.Array, Signature.Offset, Message.Array, Message.Offset, Message.Count, ref internalKey);
        }

        [Obsolete("Needs more testing")]
        public override bool Verify(byte[] Signature, byte[] Message, byte[] Key)
        {
            if (Signature == null)
                throw new ArgumentNullException(nameof(Signature));
            if (Message == null)
                throw new ArgumentNullException(nameof(Message));
            if (Key == null)
                throw new ArgumentNullException(nameof(Key));
            if (Signature.Length != 16)
                throw new ArgumentException("Invalid signature size", nameof(Signature));
            if (Key.Length != 32)
                throw new ArgumentException("Invalid key size", nameof(Key));

            var tempBytes = new byte[16];//todo: remove allocation
            Array8<UInt32> internalKey;
            ByteIntegerConverter.Array8LoadLittleEndian32(out internalKey, Key, 0);
            Poly1305Donna.poly1305_auth(tempBytes, 0, Message, 0, Message.Length, ref internalKey);
            return CryptoBytes.ConstantTimeEquals(tempBytes, Signature);
        }

        [Obsolete("Needs more testing")]
        public override bool Verify(ArraySegment<byte> Signature, ArraySegment<byte> Message, ArraySegment<byte> Key)
        {
            if (Signature.Array == null)
                throw new ArgumentNullException(nameof(Signature));
            if (Message.Array == null)
                throw new ArgumentNullException(nameof(Message));
            if (Key.Array == null)
                throw new ArgumentNullException(nameof(Key));
            if (Key.Count != 32)
                throw new ArgumentException("Invalid key size", nameof(Key));
            if (Signature.Count != 16)
                throw new ArgumentException("Invalid signature size", nameof(Signature));

            var tempBytes = new byte[16];//todo: remove allocation
            Array8<UInt32> internalKey;
            ByteIntegerConverter.Array8LoadLittleEndian32(out internalKey, Key.Array, Key.Offset);
            Poly1305Donna.poly1305_auth(tempBytes, 0, Message.Array, Message.Offset, Message.Count, ref internalKey);
            return CryptoBytes.ConstantTimeEquals(new ArraySegment<byte>(tempBytes), Signature);
        }
    }
}

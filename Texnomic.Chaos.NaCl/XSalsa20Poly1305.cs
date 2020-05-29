using System;
using Texnomic.Chaos.NaCl.Internal;
using Texnomic.Chaos.NaCl.Internal.Salsa;

namespace Texnomic.Chaos.NaCl
{
    public static class XSalsa20Poly1305
    {
        public static readonly int KeySizeInBytes = 32;
        public static readonly int NonceSizeInBytes = 24;
        public static readonly int MacSizeInBytes = 16;

        public static byte[] Encrypt(byte[] Message, byte[] Key, byte[] Nonce)
        {
            if (Message == null)
                throw new ArgumentNullException(nameof(Message));
            if (Key == null)
                throw new ArgumentNullException(nameof(Key));
            if (Nonce == null)
                throw new ArgumentNullException(nameof(Nonce));
            if (Key.Length != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (Nonce.Length != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");

            var CipherText = new byte[Message.Length + MacSizeInBytes];
            EncryptInternal(CipherText, 0, Message, 0, Message.Length, Key, 0, Nonce, 0);
            return CipherText;
        }

        public static void Encrypt(ArraySegment<byte> Ciphertext, ArraySegment<byte> Message, ArraySegment<byte> Key, ArraySegment<byte> Nonce)
        {
            if (Key.Count != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (Nonce.Count != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");
            if (Ciphertext.Count != Message.Count + MacSizeInBytes)
                throw new ArgumentException("ciphertext.Count != message.Count + 16");
            EncryptInternal(Ciphertext.Array, Ciphertext.Offset, Message.Array, Message.Offset, Message.Count, Key.Array, Key.Offset, Nonce.Array, Nonce.Offset);
        }

        /// <summary>
        /// Decrypts the ciphertext and verifies its authenticity
        /// </summary>
        /// <returns>Plaintext if MAC validation succeeds, null if the data is invalid.</returns>
        public static byte[] TryDecrypt(byte[] Ciphertext, byte[] Key, byte[] Nonce)
        {
            if (Ciphertext == null)
                throw new ArgumentNullException(nameof(Ciphertext));
            if (Key == null)
                throw new ArgumentNullException(nameof(Key));
            if (Nonce == null)
                throw new ArgumentNullException(nameof(Nonce));
            if (Key.Length != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (Nonce.Length != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");

            if (Ciphertext.Length < MacSizeInBytes)
                return null;
            var plaintext = new byte[Ciphertext.Length - MacSizeInBytes];
            var success = DecryptInternal(plaintext, 0, Ciphertext, 0, Ciphertext.Length, Key, 0, Nonce, 0);
            if (success)
                return plaintext;
            else
                return null;
        }

        /// <summary>
        /// Decrypts the ciphertext and verifies its authenticity
        /// </summary>
        /// <param name="Message">Plaintext if authentication succeeded, all zero if authentication failed, unmodified if argument verification fails</param>
        /// <param name="Ciphertext"></param>
        /// <param name="Key">Symmetric key. Must be identical to key specified for encryption.</param>
        /// <param name="Nonce">Must be identical to nonce specified for encryption.</param>
        /// <returns>true if ciphertext is authentic, false otherwise</returns>
        public static bool TryDecrypt(ArraySegment<byte> Message, ArraySegment<byte> Ciphertext, ArraySegment<byte> Key, ArraySegment<byte> Nonce)
        {
            if (Key.Count != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (Nonce.Count != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");
            if (Ciphertext.Count != Message.Count + MacSizeInBytes)
                throw new ArgumentException("ciphertext.Count != message.Count + 16");

            return DecryptInternal(Message.Array, Message.Offset, Ciphertext.Array, Ciphertext.Offset, Ciphertext.Count, Key.Array, Key.Offset, Nonce.Array, Nonce.Offset);
        }

        private static void PrepareInternalKey(out Array16<UInt32> InternalKey, byte[] Key, int KeyOffset, byte[] Nonce, int NonceOffset)
        {
            InternalKey.x0 = Salsa20.SalsaConst0;
            InternalKey.x1 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 0);
            InternalKey.x2 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 4);
            InternalKey.x3 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 8);
            InternalKey.x4 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 12);
            InternalKey.x5 = Salsa20.SalsaConst1;
            InternalKey.x6 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 0);
            InternalKey.x7 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 4);
            InternalKey.x8 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 8);
            InternalKey.x9 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 12);
            InternalKey.x10 = Salsa20.SalsaConst2;
            InternalKey.x11 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 16);
            InternalKey.x12 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 20);
            InternalKey.x13 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 24);
            InternalKey.x14 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 28);
            InternalKey.x15 = Salsa20.SalsaConst3;
            SalsaCore.HSalsa(out InternalKey, ref InternalKey, 20);

            //key
            InternalKey.x1 = InternalKey.x0;
            InternalKey.x2 = InternalKey.x5;
            InternalKey.x3 = InternalKey.x10;
            InternalKey.x4 = InternalKey.x15;
            InternalKey.x11 = InternalKey.x6;
            InternalKey.x12 = InternalKey.x7;
            InternalKey.x13 = InternalKey.x8;
            InternalKey.x14 = InternalKey.x9;
            //const
            InternalKey.x0 = Salsa20.SalsaConst0;
            InternalKey.x5 = Salsa20.SalsaConst1;
            InternalKey.x10 = Salsa20.SalsaConst2;
            InternalKey.x15 = Salsa20.SalsaConst3;
            //nonce
            InternalKey.x6 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 16);
            InternalKey.x7 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 20);
            //offset
            InternalKey.x8 = 0;
            InternalKey.x9 = 0;
        }

        private static bool DecryptInternal(byte[] Plaintext, int PlaintextOffset, byte[] Ciphertext, int CiphertextOffset, int CiphertextLength, byte[] Key, int KeyOffset, byte[] Nonce, int NonceOffset)
        {
            var plaintextLength = CiphertextLength - MacSizeInBytes;
            Array16<UInt32> internalKey;
            PrepareInternalKey(out internalKey, Key, KeyOffset, Nonce, NonceOffset);

            Array16<UInt32> temp;
            var tempBytes = new byte[64];//todo: remove allocation

            // first iteration
            {
                SalsaCore.Salsa(out temp, ref internalKey, 20);

                //first half is for Poly1305
                Array8<UInt32> poly1305Key;
                poly1305Key.x0 = temp.x0;
                poly1305Key.x1 = temp.x1;
                poly1305Key.x2 = temp.x2;
                poly1305Key.x3 = temp.x3;
                poly1305Key.x4 = temp.x4;
                poly1305Key.x5 = temp.x5;
                poly1305Key.x6 = temp.x6;
                poly1305Key.x7 = temp.x7;

                // compute MAC
                Poly1305Donna.poly1305_auth(tempBytes, 0, Ciphertext, CiphertextOffset + 16, plaintextLength, ref poly1305Key);
                if (!CryptoBytes.ConstantTimeEquals(tempBytes, 0, Ciphertext, CiphertextOffset, MacSizeInBytes))
                {
                    Array.Clear(Plaintext, PlaintextOffset, plaintextLength);
                    return false;
                }

                // rest for the message
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 0, temp.x8);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 4, temp.x9);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 8, temp.x10);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 12, temp.x11);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 16, temp.x12);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 20, temp.x13);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 24, temp.x14);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 28, temp.x15);
                var count = Math.Min(32, plaintextLength);
                for (var i = 0; i < count; i++)
                    Plaintext[PlaintextOffset + i] = (byte)(Ciphertext[MacSizeInBytes + CiphertextOffset + i] ^ tempBytes[i]);
            }

            // later iterations
            var blockOffset = 32;
            while (blockOffset < plaintextLength)
            {
                internalKey.x8++;
                SalsaCore.Salsa(out temp, ref internalKey, 20);
                ByteIntegerConverter.Array16StoreLittleEndian32(tempBytes, 0, ref temp);
                var count = Math.Min(64, plaintextLength - blockOffset);
                for (var i = 0; i < count; i++)
                    Plaintext[PlaintextOffset + blockOffset + i] = (byte)(Ciphertext[16 + CiphertextOffset + blockOffset + i] ^ tempBytes[i]);
                blockOffset += 64;
            }
            return true;
        }

        private static void EncryptInternal(byte[] Ciphertext, int CiphertextOffset, byte[] Message, int MessageOffset, int MessageLength, byte[] Key, int KeyOffset, byte[] Nonce, int NonceOffset)
        {
            Array16<UInt32> internalKey;
            PrepareInternalKey(out internalKey, Key, KeyOffset, Nonce, NonceOffset);

            Array16<UInt32> temp;
            var tempBytes = new byte[64];//todo: remove allocation
            Array8<UInt32> poly1305Key;

            // first iteration
            {
                SalsaCore.Salsa(out temp, ref internalKey, 20);

                //first half is for Poly1305
                poly1305Key.x0 = temp.x0;
                poly1305Key.x1 = temp.x1;
                poly1305Key.x2 = temp.x2;
                poly1305Key.x3 = temp.x3;
                poly1305Key.x4 = temp.x4;
                poly1305Key.x5 = temp.x5;
                poly1305Key.x6 = temp.x6;
                poly1305Key.x7 = temp.x7;

                // second half for the message
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 0, temp.x8);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 4, temp.x9);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 8, temp.x10);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 12, temp.x11);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 16, temp.x12);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 20, temp.x13);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 24, temp.x14);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 28, temp.x15);
                var count = Math.Min(32, MessageLength);
                for (var i = 0; i < count; i++)
                    Ciphertext[16 + CiphertextOffset + i] = (byte)(Message[MessageOffset + i] ^ tempBytes[i]);
            }

            // later iterations
            var blockOffset = 32;
            while (blockOffset < MessageLength)
            {
                internalKey.x8++;
                SalsaCore.Salsa(out temp, ref internalKey, 20);
                ByteIntegerConverter.Array16StoreLittleEndian32(tempBytes, 0, ref temp);
                var count = Math.Min(64, MessageLength - blockOffset);
                for (var i = 0; i < count; i++)
                    Ciphertext[16 + CiphertextOffset + blockOffset + i] = (byte)(Message[MessageOffset + blockOffset + i] ^ tempBytes[i]);
                blockOffset += 64;
            }

            // compute MAC
            Poly1305Donna.poly1305_auth(Ciphertext, CiphertextOffset, Ciphertext, CiphertextOffset + 16, MessageLength, ref poly1305Key);
        }
    }
}

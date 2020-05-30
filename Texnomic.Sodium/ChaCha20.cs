using System;
using System.Security.Cryptography;

namespace Texnomic.Sodium
{
    public static class ChaCha20
    {
        public static byte[] Hash(byte[] Data, byte[] Key, byte[] Nonce)
        {
            if (Key.Length < 32)
                throw new ArgumentException("Key Length Must Be 32 Bytes.", nameof(Key));

            if (Nonce.Length < 16)
                throw new ArgumentException("Nonce Length Must Be 16 Bytes.", nameof(Nonce));

            var Hash = new byte[Data.Length];

            var Result =  SodiumLibrary.crypto_core_hchacha20(Hash, Data, Key, Nonce);

            return Result == 0 ? Hash : throw new CryptographicException();
        }
    }
}

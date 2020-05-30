using System.Security.Cryptography;


namespace Texnomic.Sodium
{
    public static class Curve25519XChaCha20Poly1305
    {
        public static byte[] KeyExchange(byte[] PublicKey, byte[] SecretKey)
        {
            var SharedKey = new byte[32];

            var Result = SodiumLibrary.crypto_box_curve25519xchacha20poly1305_beforenm(SharedKey, PublicKey, SecretKey);

            return Result == 0 ? SharedKey : throw new CryptographicException();
        }

        public static byte[] Encrypt(byte[] Message, byte[] Nonce, byte[] SharedKey)
        {
            var CipherText = new byte[Message.Length + 16];

            var Result = SodiumLibrary.crypto_box_curve25519xchacha20poly1305_easy_afternm(CipherText, Message, Message.Length, Nonce, SharedKey);

            return Result == 0 ? CipherText : throw new CryptographicException();
        }

        public static byte[] Encrypt(byte[] Message, byte[] Nonce, byte[] PublicKey, byte[] SecretKey)
        {
            var CipherText = new byte[Message.Length + 16];

            var Result = SodiumLibrary.crypto_box_curve25519xchacha20poly1305_easy(CipherText, Message, Message.Length, Nonce, PublicKey, SecretKey);

            return Result == 0 ? CipherText : throw new CryptographicException();
        }

        public static byte[] Decrypt(byte[] CipherText, byte[] Nonce, byte[] SharedKey)
        {
            var PlainText = new byte[CipherText.Length - 16];

            var Result = SodiumLibrary.crypto_box_curve25519xchacha20poly1305_open_easy_afternm(PlainText, CipherText, CipherText.Length, Nonce, SharedKey);

            return Result == 0 ? PlainText : throw new CryptographicException();
        }

        public static byte[] Decrypt(byte[] CipherText, byte[] Nonce, byte[] PublicKey, byte[] SecretKey)
        {
            var PlainText = new byte[CipherText.Length - 16];

            var Result = SodiumLibrary.crypto_box_curve25519xchacha20poly1305_open_easy(PlainText, CipherText, CipherText.Length, Nonce, PublicKey, SecretKey);

            return Result == 0 ? PlainText : throw new CryptographicException();
        }
    }
}

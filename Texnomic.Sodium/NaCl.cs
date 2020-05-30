using System.Security.Cryptography;

namespace Texnomic.Sodium
{
    public static class NaCl
    {
        public static byte[] KeyExchange(byte[] PublicKey, byte[] SecretKey)
        {
            var SharedKey = new byte[32];

            var Result = SodiumLibrary.crypto_box_beforenm(SharedKey, PublicKey, SecretKey);

            return Result == 0 ? SharedKey : throw new CryptographicException();
        }
    }
}

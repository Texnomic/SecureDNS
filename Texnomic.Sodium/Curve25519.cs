using System.Security.Cryptography;


namespace Texnomic.Sodium
{
    /// <summary>
    /// Point*scalar Multiplication
    /// </summary>
    /// <see cref="https://libsodium.gitbook.io/doc/advanced/scalar_multiplication"/>
    public static class Curve25519
    {
        /// <summary>
        /// Given a user's secret key, the function computes the user's public key.
        /// </summary>
        /// <see cref="https://libsodium.gitbook.io/doc/advanced/scalar_multiplication#usage"/>
        public static byte[] ScalarMultiplicationBase(byte[] SecretKey)
        {
            var PublicKey = new byte[SecretKey.Length];

            var Result = SodiumLibrary.crypto_scalarmult_base(PublicKey, SecretKey);

            return Result == 0 ? PublicKey : throw new CryptographicException();
        }

        /// <summary>
        /// This function can be used to compute a shared secret given a user's secret key and another user's public key.
        /// </summary>
        /// <see cref="https://libsodium.gitbook.io/doc/advanced/scalar_multiplication#usage"/>
        public static byte[] ScalarMultiplication(byte[] SecretKey, byte[] PublicKey)
        {
            var SharedKey = new byte[SecretKey.Length];

            var Result = SodiumLibrary.crypto_scalarmult(SharedKey, SecretKey, PublicKey);

            return Result == 0 ? SharedKey : throw new CryptographicException();
        }
    }
}

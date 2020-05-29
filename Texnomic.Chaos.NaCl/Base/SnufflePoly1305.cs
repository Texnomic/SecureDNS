using System;
using System.Security.Cryptography;

namespace Texnomic.Chaos.NaCl.Base
{
    /// <summary>
    /// An AEAD construction with a <see cref="Base.Snuffle"/> and <see cref="Poly1305"/>, following RFC 8439, section 2.8.
    ///
    /// This implementation produces ciphertext with the following format: {nonce || actual_ciphertext || tag} and only decrypts the same format.
    /// </summary>
    public abstract class SnufflePoly1305
    {
        private readonly Snuffle Snuffle;
        private readonly Snuffle MacKeySnuffle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnufflePoly1305"/> class.
        /// </summary>
        /// <param name="Key">The secret key.</param>
        public SnufflePoly1305(ReadOnlyMemory<byte> Key)
        {
            Snuffle = CreateSnuffleInstance(Key, 1);
            MacKeySnuffle = CreateSnuffleInstance(Key, 0);
        }

        /// <summary>
        /// Creates the snuffle instance.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="InitialCounter">The initial counter.</param>
        /// <returns>Snuffle.</returns>
        protected abstract Snuffle CreateSnuffleInstance(ReadOnlyMemory<byte> Key, int InitialCounter);

        /// <summary>
        /// Encrypts the <paramref name="Plaintext"> into the <paramref name="Ciphertext"> destination buffer and computes an authentication tag into a separate buffer with <see cref="Poly1305"/> authentication based on an <paramref name="associatedData"> and a <paramref name="Nonce">.
        /// </summary>
        /// <param name="Nonce">The nonce associated with this message, which should be a unique value for every operation with the same key.</param>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="Ciphertext">The byte array to receive the encrypted contents.</param>
        /// <param name="tag">The byte array to receive the generated authentication tag.</param>
        /// <param name="associatedData">Extra data associated with this message, which must also be provided during decryption.</param>
        /// <exception cref="CryptographicException">plaintext or nonce</exception>
        public void Encrypt(byte[] Nonce, byte[] Plaintext, byte[] Ciphertext) => Snuffle.Encrypt(Plaintext, Nonce, Ciphertext);


        /// <summary>
        /// Decrypts the <paramref name="Ciphertext"> into the <paramref name="Plaintext"> provided destination buffer if the authentication <paramref name="tag"> can be validated.
        /// </summary>
        /// <param name="Nonce">The nonce associated with this message, which must match the value provided during encryption.</param>
        /// <param name="Ciphertext">The encrypted content to decrypt.</param>
        /// <param name="tag">The authentication tag produced for this message during encryption.</param>
        /// <param name="Plaintext">The byte array to receive the decrypted contents.</param>
        /// <param name="associatedData">Extra data associated with this message, which must match the value provided during encryption.</param>
        /// <exception cref="CryptographicException">The tag value could not be verified, or the decryption operation otherwise failed.</exception>
        public virtual void Decrypt(byte[] Nonce, byte[] Ciphertext, byte[] Plaintext) => Snuffle.Decrypt(Ciphertext, Nonce, Plaintext);
    }
}

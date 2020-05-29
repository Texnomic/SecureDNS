using System;
using System.Security.Cryptography;
using Texnomic.Chaos.NaCl.Base;
using Texnomic.Chaos.NaCl.Internal;

namespace Texnomic.Chaos.NaCl
{
    /// <summary>
    /// A stream cipher based on https://download.libsodium.org/doc/advanced/xchacha20.html and https://tools.ietf.org/html/draft-arciszewski-xchacha-02.
    ///
    /// This cipher is meant to be used to construct an AEAD with Poly1305.
    /// </summary>
    /// <seealso cref="ChaCha20Base" />
    public class XChaCha20 : ChaCha20Base
    {
        public const int NONCE_SIZE_IN_BYTES = 24;

        /// <summary>
        /// Initializes a new instance of the <see cref="XChaCha20"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="initialCounter">The initial counter.</param>
        public XChaCha20(ReadOnlyMemory<byte> key, int initialCounter) : base(key, initialCounter)
        {
        }

        /// <inheritdoc />
        protected override void SetInitialState(Span<uint> state, ReadOnlySpan<byte> nonce, int counter)
        {
            if (nonce.IsEmpty || nonce.Length != NonceSizeInBytes)
                throw new CryptographicException(
                    FormatNonceLengthExceptionMessage(GetType().Name, nonce.Length, NonceSizeInBytes));

            // The first four words (0-3) are constants: 0x61707865, 0x3320646e, 0x79622d32, 0x6b206574.
            SetSigma(state);

            // The next eight words (4-11) are taken from the 256-bit key in little-endian order, in 4-byte chunks; and the first 16 bytes of the 24-byte nonce to obtain the subkey.
            Span<byte> subKey = stackalloc byte[KeySizeInBytes];
            HChaCha20(subKey, nonce);
            SetKey(state, subKey);

            // Word 12 is a block counter.
            state[12] = (uint) counter;

            // Word 13 is a prefix of 4 null bytes, since RFC 8439 specifies a 12-byte nonce.
            state[13] = 0;

            // Words 14-15 are the remaining 8-byte nonce (used in HChaCha20). Ref: https://tools.ietf.org/html/draft-arciszewski-xchacha-01#section-2.3.
            state[14] = ArrayUtils.LoadUInt32LittleEndian(nonce, 16);
            state[15] = ArrayUtils.LoadUInt32LittleEndian(nonce, 20);
        }

        /// <summary>
        /// The size of the randomly generated nonces.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public override int NonceSizeInBytes => NONCE_SIZE_IN_BYTES;
    }
}

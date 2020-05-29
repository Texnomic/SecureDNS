using System;
using System.Buffers;
using System.Security.Cryptography;

namespace Texnomic.Chaos.NaCl.Base
{
    /// <summary>
    /// Abstract base class for XSalsa20, ChaCha20, XChaCha20 and their variants.
    /// </summary>
    /// <remarks>
    /// Variants of Snuffle have two differences: the size of the nonce and the block function that
    /// produces a key stream block from a key, a nonce, and a counter. Subclasses of this class
    /// specifying these two information by overriding <seealso cref="NaCl.Core.Base.Snuffle.NonceSizeInBytes()" /> and <seealso cref="NaCl.Core.Base.Snuffle.GetKeyStreamBlock(byte[], int)" />.
    ///
    /// Concrete implementations of this class are meant to be used to construct an Aead with <seealso cref="NaCl.Core.Poly1305" />. The
    /// base class of these Aead constructions is <seealso cref="NaCl.Core.Base.SnufflePoly1305" />.
    /// For example, <seealso cref="NaCl.Core.XChaCha20" /> is a subclass of this class and a
    /// concrete Snuffle implementation, and <seealso cref="NaCl.Core.XChaCha20Poly1305" /> is
    /// a subclass of <seealso cref="NaCl.Core.Base.SnufflePoly1305" /> and a concrete Aead construction.
    /// </remarks>
    public abstract class Snuffle
    {
        public const int BlockSizeInInts = 16;
        public static int BlockSizeInBytes = BlockSizeInInts * 4; // 64
        public const int KeySizeInInts = 8;
        public static int KeySizeInBytes = KeySizeInInts * 4; // 32

        public static uint[] Sigma = new uint[] { 0x61707865, 0x3320646E, 0x79622D32, 0x6B206574 }; //Encoding.ASCII.GetBytes("expand 32-byte k");

        protected readonly ReadOnlyMemory<byte> Key;
        protected readonly int InitialCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Snuffle"/> class.
        /// </summary>
        /// <param name="Key">The secret key.</param>
        /// <param name="InitialCounter">The initial counter.</param>
        /// <exception cref="CryptographicException"></exception>
        public Snuffle(ReadOnlyMemory<byte> Key, int InitialCounter)
        {
            if (Key.Length != KeySizeInBytes)
                throw new CryptographicException($"The key length in bytes must be {KeySizeInBytes}.");

            this.Key = Key;
            this.InitialCounter = InitialCounter;
        }

        /// <summary>
        /// Process the keystream block <paramref name="Block"> from <paramref name="Nonce"> and <paramref name="Counter">.
        ///
        /// From this function, the Snuffle encryption function can be constructed using the counter
        /// mode of operation. For example, the ChaCha20 block function and how it can be used to
        /// construct the ChaCha20 encryption function are described in section 2.3 and 2.4 of RFC 8439.
        /// </summary>
        /// <param name="Nonce">The nonce.</param>
        /// <param name="Counter">The counter.</param>
        /// <param name="Block">The stream block.</param>
        /// <returns>ByteBuffer.</returns>
        public abstract void ProcessKeyStreamBlock(ReadOnlySpan<byte> Nonce, int Counter, Span<byte> Block);

        /// <summary>
        /// The size of the randomly generated nonces.
        /// ChaCha20 uses 12-byte nonces, but XSalsa20 and XChaCha20 use 24-byte nonces.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public abstract int NonceSizeInBytes { get; }

        /// <summary>
        /// Encrypts the <paramref name="Plaintext"> using an unique generated nonce.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <returns>The encrypted contents.</returns>
        /// <exception cref="CryptographicException">plaintext or ciphertext</exception>
        public virtual byte[] Encrypt(byte[] Plaintext) => Encrypt((ReadOnlySpan<byte>)Plaintext);

        /// <summary>
        /// Encrypts the <paramref name="Plaintext"> using an unique generated nonce.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <returns>The encrypted contents.</returns>
        /// <exception cref="CryptographicException">plaintext or ciphertext</exception>
        public virtual byte[] Encrypt(ReadOnlySpan<byte> Plaintext)
        {
            //if (plaintext.Length > int.MaxValue - NonceSizeInBytes())
            //    throw new ArgumentException($"The {nameof(plaintext)} is too long.");

            var ciphertext = new byte[Plaintext.Length + NonceSizeInBytes];

#if NETCOREAPP3_1
            Span<byte> nonce = stackalloc byte[NonceSizeInBytes];
            RandomNumberGenerator.Fill(nonce);

            nonce.CopyTo(ciphertext);
#else
            var nonce = new byte[NonceSizeInBytes];
            RandomNumberGenerator.Create().GetBytes(nonce);

            Array.Copy(nonce, ciphertext, nonce.Length);
#endif

            Process(nonce, ciphertext, Plaintext, nonce.Length);

            return ciphertext;
        }

        /// <summary>
        /// Encrypts the <paramref name="Plaintext"> using the associated <paramref name="Nonce">.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="Nonce">The nonce associated with this message, which should be a unique value for every operation with the same key.</param>
        /// <returns>The encrypted contents.</returns>
        /// <exception cref="CryptographicException">plaintext or nonce</exception>
        public virtual byte[] Encrypt(byte[] Plaintext, byte[] Nonce) => Encrypt((ReadOnlySpan<byte>)Plaintext, (ReadOnlySpan<byte>)Nonce);

        /// <summary>
        /// Encrypts the <paramref name="Plaintext"> using the associated <paramref name="Nonce">.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="Nonce">The nonce associated with this message, which should be a unique value for every operation with the same key.</param>
        /// <returns>The encrypted contents.</returns>
        /// <exception cref="CryptographicException">plaintext or nonce</exception>
        public virtual byte[] Encrypt(ReadOnlySpan<byte> Plaintext, ReadOnlySpan<byte> Nonce)
        {
            var ciphertext = new byte[Plaintext.Length];
            Encrypt(Plaintext, Nonce, ciphertext);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts the <paramref name="Plaintext"> into the <paramref name="Ciphertext"> destination buffer using the associated <paramref name="Nonce">.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="Nonce">The nonce associated with this message, which should be a unique value for every operation with the same key.</param>
        /// <param name="Ciphertext">The byte array to receive the encrypted contents.</param>
        /// <exception cref="CryptographicException">plaintext or nonce</exception>
        public virtual void Encrypt(ReadOnlySpan<byte> Plaintext, ReadOnlySpan<byte> Nonce, Span<byte> Ciphertext)
        {
            //if (plaintext.Length > int.MaxValue - NonceSizeInBytes())
            //    throw new ArgumentException($"The {nameof(plaintext)} is too long.");

            if (Plaintext.Length != Ciphertext.Length)
                throw new ArgumentException("The plaintext parameter and the ciphertext do not have the same length.");

            if (Nonce.IsEmpty || Nonce.Length != NonceSizeInBytes)
                throw new ArgumentException(FormatNonceLengthExceptionMessage(GetType().Name, Nonce.Length, NonceSizeInBytes));

            Process(Nonce, Ciphertext, Plaintext);
        }

        /// <summary>
        /// Decrypts the specified ciphertext.
        /// </summary>
        /// <param name="Ciphertext">The encrypted content to decrypt.</param>
        /// <returns>The decrypted contents.</returns>
        /// <exception cref="CryptographicException">ciphertext</exception>
        public virtual byte[] Decrypt(ReadOnlySpan<byte> Ciphertext)
        {
            if (Ciphertext.Length < NonceSizeInBytes)
                throw new ArgumentException($"The {nameof(Ciphertext)} is too short.");

            var plaintext = new byte[Ciphertext.Length - NonceSizeInBytes];
            Decrypt(Ciphertext.Slice(NonceSizeInBytes), Ciphertext.Slice(0, NonceSizeInBytes), plaintext); //Process(ciphertext.Slice(0, NonceSizeInBytes), plaintext, ciphertext.Slice(NonceSizeInBytes));
            return plaintext;
        }

        /// <summary>
        /// Decrypts the <paramref name="Ciphertext"> using the associated <paramref name="Nonce">.
        /// </summary>
        /// <param name="Ciphertext">The encrypted content to decrypt.</param>
        /// <param name="Nonce">The nonce associated with this message, which must match the value provided during encryption.</param>
        /// <returns>The decrypted contents.</returns>
        /// <exception cref="CryptographicException">ciphertext or nonce</exception>
        public virtual byte[] Decrypt(ReadOnlySpan<byte> Ciphertext, ReadOnlySpan<byte> Nonce)
        {
            var plaintext = new byte[Ciphertext.Length];
            Decrypt(Ciphertext, Nonce, plaintext);
            return plaintext;
        }

        /// <summary>
        /// Decrypts the <paramref name="Ciphertext"> into the <paramref name="Plaintext"> provided destination buffer using the associated <paramref name="Nonce">.
        /// </summary>
        /// <param name="Ciphertext">The encrypted content to decrypt.</param>
        /// <param name="Nonce">The nonce associated with this message, which must match the value provided during encryption.</param>
        /// <param name="Plaintext">The byte span to receive the decrypted contents.</param>
        /// <exception cref="CryptographicException">ciphertext or nonce.</exception>
        public virtual void Decrypt(ReadOnlySpan<byte> Ciphertext, ReadOnlySpan<byte> Nonce, Span<byte> Plaintext)
        {
            if (Nonce.IsEmpty || Nonce.Length != NonceSizeInBytes)
                throw new ArgumentException(FormatNonceLengthExceptionMessage(GetType().Name, Nonce.Length, NonceSizeInBytes));

            Process(Nonce, Plaintext, Ciphertext);
        }

        /// <summary>
        /// Processes the Encryption/Decryption function.
        /// </summary>
        /// <param name="Nonce">The nonce.</param>
        /// <param name="Output">The output.</param>
        /// <param name="Input">The input.</param>
        /// <param name="Offset">The output's starting offset.</param>
        private void Process(ReadOnlySpan<byte> Nonce, Span<byte> Output, ReadOnlySpan<byte> Input, int Offset = 0)
        {
            var length = Input.Length;
            var numBlocks = (length / BlockSizeInBytes) + 1;

            /*
             * Allocates 64 bytes more than below impl as per the benchmarks...
             * 
            var block = new byte[BLOCK_SIZE_IN_BYTES];
            for (var i = 0; i < numBlocks; i++)
            {
                ProcessKeyStreamBlock(nonce, i + InitialCounter, block);

                if (i == numBlocks - 1)
                    Xor(output, input, block, length % BLOCK_SIZE_IN_BYTES, offset, i); // last block
                else
                    Xor(output, input, block, BLOCK_SIZE_IN_BYTES, offset, i);

                CryptoBytes.Wipe(block); // Array.Clear(block, 0, block.Length);
            }
            */

            using (var owner = MemoryPool<byte>.Shared.Rent(BlockSizeInBytes))
            {
                for (var i = 0; i < numBlocks; i++)
                {
                    ProcessKeyStreamBlock(Nonce, i + InitialCounter, owner.Memory.Span);

                    if (i == numBlocks - 1)
                        Xor(Output, Input, owner.Memory.Span, length % BlockSizeInBytes, Offset, i); // last block
                    else
                        Xor(Output, Input, owner.Memory.Span, BlockSizeInBytes, Offset, i);

                    owner.Memory.Span.Clear();
                }
            }
        }

        /// <summary>
        /// Formats the nonce length exception message.
        /// </summary>
        /// <param name="Name">The crypto primitive name.</param>
        /// <param name="Actual">The actual nonce length.</param>
        /// <param name="Expected">The expected nonce length.</param>
        /// <returns>System.String.</returns>
        internal string FormatNonceLengthExceptionMessage(string Name, int Actual, int Expected) => $"{Name} uses {Expected * 8}-bit nonces, but got a {Actual * 8}-bit nonce. The nonce length in bytes must be {Expected}.";

        /// <summary>
        /// XOR the specified output.
        /// </summary>
        /// <param name="Output">The output.</param>
        /// <param name="Input">The input.</param>
        /// <param name="Block">The key stream block.</param>
        /// <param name="Len">The length.</param>
        /// <param name="Offset">The output's starting offset.</param>
        /// <param name="CurBlock">The current block number.</param>
        /// <exception cref="CryptographicException">The combination of blocks, offsets and length to be XORed is out-of-bonds.</exception>
        private static void Xor(Span<byte> Output, ReadOnlySpan<byte> Input, ReadOnlySpan<byte> Block, int Len, int Offset, int CurBlock)
        {
            var blockOffset = CurBlock * BlockSizeInBytes;

            if (Len < 0 || Offset < 0 || CurBlock < 0 || Output.Length < Len || (Input.Length - blockOffset) < Len || Block.Length < Len)
                throw new CryptographicException("The combination of blocks, offsets and length to be XORed is out-of-bonds.");

            for (var i = 0; i < Len; i++)
                Output[i + Offset + blockOffset] = (byte)(Input[i + blockOffset] ^ Block[i]);
        }
    }
}

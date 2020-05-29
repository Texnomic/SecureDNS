using System;
using System.Security.Cryptography;
using Texnomic.Chaos.NaCl.Internal;

namespace Texnomic.Chaos.NaCl.Base
{
    /// <summary>
    /// Base class for <seealso cref="ChaCha20" /> and <seealso cref="XChaCha20" />.
    /// </summary>
    /// <seealso cref="Snuffle" />
    public abstract class ChaCha20Base : Snuffle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaCha20Base"/> class.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="InitialCounter">The initial counter.</param>
        public ChaCha20Base(ReadOnlyMemory<byte> Key, int InitialCounter) : base(Key, InitialCounter) { }

        /// <summary>
        /// Sets the initial <paramref name="State"/> from <paramref name="Nonce"/> and <paramref name="Counter">.
        /// ChaCha20 has a different logic than XChaCha20, because the former uses a 12-byte nonce, but the later uses 24-byte.
        /// </summary>
        /// <param name="State">The state.</param>
        /// <param name="Nonce">The nonce.</param>
        /// <param name="Counter">The counter.</param>
        protected abstract void SetInitialState(Span<uint> State, ReadOnlySpan<byte> Nonce, int Counter);

        /// <inheritdoc />
        public override void ProcessKeyStreamBlock(ReadOnlySpan<byte> Nonce, int Counter, Span<byte> Block)
        {
            if (Block.Length != BlockSizeInBytes)
                throw new CryptographicException($"The keystream block length is not valid. The length in bytes must be {BlockSizeInBytes}.");

            // Set the initial state based on https://tools.ietf.org/html/rfc8439#section-2.3
            Span<uint> state = stackalloc uint[BlockSizeInInts];
            SetInitialState(state, Nonce, Counter);

            // Create a copy of the state and then run 20 rounds on it,
            // alternating between "column rounds" and "diagonal rounds"; each round consisting of four quarter-rounds.
            Span<uint> workingState = stackalloc uint[BlockSizeInInts];
            state.CopyTo(workingState);
            ShuffleState(workingState);

            // At the end of the rounds, add the result to the original state.
            state[0] += workingState[0];
            state[1] += workingState[1];
            state[2] += workingState[2];
            state[3] += workingState[3];
            state[4] += workingState[4];
            state[5] += workingState[5];
            state[6] += workingState[6];
            state[7] += workingState[7];
            state[8] += workingState[8];
            state[9] += workingState[9];
            state[10] += workingState[10];
            state[11] += workingState[11];
            state[12] += workingState[12];
            state[13] += workingState[13];
            state[14] += workingState[14];
            state[15] += workingState[15];

            ArrayUtils.StoreArray16UInt32LittleEndian(Block, 0, state);
        }

        /// <summary>
        /// Process a pseudorandom keystream block, converting the key and part of the <paramref name="Nonce"> into a <paramref name="subkey">, and the remainder of the <paramref name="Nonce">.
        /// </summary>
        /// <param name="SubKey">The subKey.</param>
        /// <param name="Nonce">The nonce.</param>
        public void HChaCha20(Span<byte> SubKey, ReadOnlySpan<byte> Nonce)
        {
            // See https://tools.ietf.org/html/draft-arciszewski-xchacha-01#section-2.2.

            Span<uint> state = stackalloc uint[BlockSizeInInts];

            // Set ChaCha20 constant
            SetSigma(state);

            // Set 256-bit Key
            SetKey(state, Key.Span);

            // Set 128-bit Nonce
            state[12] = ArrayUtils.LoadUInt32LittleEndian(Nonce, 0);
            state[13] = ArrayUtils.LoadUInt32LittleEndian(Nonce, 4);
            state[14] = ArrayUtils.LoadUInt32LittleEndian(Nonce, 8);
            state[15] = ArrayUtils.LoadUInt32LittleEndian(Nonce, 12);

            // Block function
            ShuffleState(state);

            state[4] = state[12];
            state[5] = state[13];
            state[6] = state[14];
            state[7] = state[15];

            ArrayUtils.StoreArray8UInt32LittleEndian(SubKey, 0, state);
        }


        protected static void ShuffleState(Span<uint> State)
        {
            for (var i = 0; i < 10; i++)
            {
                QuarterRound(ref State[0], ref State[4], ref State[8], ref State[12]);
                QuarterRound(ref State[1], ref State[5], ref State[9], ref State[13]);
                QuarterRound(ref State[2], ref State[6], ref State[10], ref State[14]);
                QuarterRound(ref State[3], ref State[7], ref State[11], ref State[15]);
                QuarterRound(ref State[0], ref State[5], ref State[10], ref State[15]);
                QuarterRound(ref State[1], ref State[6], ref State[11], ref State[12]);
                QuarterRound(ref State[2], ref State[7], ref State[8], ref State[13]);
                QuarterRound(ref State[3], ref State[4], ref State[9], ref State[14]);
            }
        }

        public static void QuarterRound(ref uint A, ref uint B, ref uint C, ref uint D)
        {
            A += B;
            D = BitUtils.RotateLeft(D ^ A, 16);
            C += D;
            B = BitUtils.RotateLeft(B ^ C, 12);
            A += B;
            D = BitUtils.RotateLeft(D ^ A, 8);
            C += D;
            B = BitUtils.RotateLeft(B ^ C, 7);
        }

        protected static void SetSigma(Span<uint> State)
        {
            State[0] = Sigma[0];
            State[1] = Sigma[1];
            State[2] = Sigma[2];
            State[3] = Sigma[3];
        }

        protected static void SetKey(Span<uint> State, ReadOnlySpan<byte> Key)
        {
            State[4] = ArrayUtils.LoadUInt32LittleEndian(Key, 0);
            State[5] = ArrayUtils.LoadUInt32LittleEndian(Key, 4);
            State[6] = ArrayUtils.LoadUInt32LittleEndian(Key, 8);
            State[7] = ArrayUtils.LoadUInt32LittleEndian(Key, 12);
            State[8] = ArrayUtils.LoadUInt32LittleEndian(Key, 16);
            State[9] = ArrayUtils.LoadUInt32LittleEndian(Key, 20);
            State[10] = ArrayUtils.LoadUInt32LittleEndian(Key, 24);
            State[11] = ArrayUtils.LoadUInt32LittleEndian(Key, 28);
        }
    }
}

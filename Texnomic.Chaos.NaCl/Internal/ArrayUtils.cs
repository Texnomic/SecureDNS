using System;
using System.Buffers.Binary;

namespace Texnomic.Chaos.NaCl.Internal
{
    internal static class ArrayUtils
    {
        #region Individual

        /// <summary>
        /// Loads 4 bytes of the input buffer into an unsigned 32-bit integer, beginning at the input offset.
        /// </summary>
        /// <param name="Buf">The input buffer.</param>
        /// <param name="Offset">The input offset.</param>
        /// <returns>System.UInt32.</returns>
        public static uint LoadUInt32LittleEndian(ReadOnlySpan<byte> Buf, int Offset)
            => BinaryPrimitives.ReadUInt32LittleEndian(Buf.Slice(Offset + 0, sizeof(int)));

        /// <summary>
        /// Stores the value into the buffer.
        /// The value will be split into 4 bytes and put into four sequential places in the output buffer, starting at the specified offset.
        /// </summary>
        /// <param name="Buf">The output buffer.</param>
        /// <param name="Offset">The output offset.</param>
        /// <param name="Value">The input value.</param>
        public static void StoreUi32LittleEndian(Span<byte> Buf, int Offset, uint Value)
            => BinaryPrimitives.WriteUInt32LittleEndian(Buf.Slice(Offset + 0, sizeof(int)), Value);

        /// <summary>
        /// Stores the value into the buffer.
        /// The value will be split into 8 bytes and put into eight sequential places in the output buffer, starting at the specified offset.
        /// </summary>
        /// <param name="Buf">The output buffer.</param>
        /// <param name="Offset">The output offset.</param>
        /// <param name="Value">The input value.</param>
        public static void StoreUInt64LittleEndian(Span<byte> Buf, int Offset, ulong Value)
            => BinaryPrimitives.WriteUInt64LittleEndian(Buf.Slice(Offset + 0, sizeof(ulong)), Value);

        #endregion

        #region Array

        public static void StoreArray8UInt32LittleEndian(Span<byte> Output, int Offset, ReadOnlySpan<uint> Input)
        {
            var len = sizeof(int);

            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 0, len), Input[0]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 4, len), Input[1]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 8, len), Input[2]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 12, len), Input[3]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 16, len), Input[4]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 20, len), Input[5]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 24, len), Input[6]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 28, len), Input[7]);
        }

        public static void StoreArray16UInt32LittleEndian(Span<byte> Output, int Offset, ReadOnlySpan<uint> Input)
        {
            var len = sizeof(int);

            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 0, len), Input[0]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 4, len), Input[1]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 8, len), Input[2]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 12, len), Input[3]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 16, len), Input[4]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 20, len), Input[5]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 24, len), Input[6]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 28, len), Input[7]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 32, len), Input[8]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 36, len), Input[9]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 40, len), Input[10]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 44, len), Input[11]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 48, len), Input[12]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 52, len), Input[13]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 56, len), Input[14]);
            BinaryPrimitives.WriteUInt32LittleEndian(Output.Slice(Offset + 60, len), Input[15]);
        }

        #endregion
    }
}

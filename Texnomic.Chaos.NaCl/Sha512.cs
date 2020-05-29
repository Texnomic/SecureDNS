using System;
using Texnomic.Chaos.NaCl.Internal;

namespace Texnomic.Chaos.NaCl
{
    public class Sha512
    {
        private Array8<UInt64> State;
        private readonly byte[] Buffer;
        private ulong TotalBytes;
        public const int BlockSize = 128;
        private static readonly byte[] Padding = new byte[] { 0x80 };

        public Sha512()
        {
            Buffer = new byte[BlockSize];//todo: remove allocation
            Init();
        }

        public void Init()
        {
            Sha512Internal.Sha512Init(out State);
            TotalBytes = 0;
        }

        public void Update(ArraySegment<byte> Data)
        {
            if (Data.Array == null)
                throw new ArgumentNullException("data.Array");
            Update(Data.Array, Data.Offset, Data.Count);
        }

        public void Update(byte[] Data, int Offset, int Count)
        {
            if (Data == null)
                throw new ArgumentNullException(nameof(Data));
            if (Offset < 0)
                throw new ArgumentOutOfRangeException(nameof(Offset));
            if (Count < 0)
                throw new ArgumentOutOfRangeException(nameof(Count));
            if (Data.Length - Offset < Count)
                throw new ArgumentException("Requires offset + count <= data.Length");

            Array16<ulong> block;
            var bytesInBuffer = (int)TotalBytes & (BlockSize - 1);
            TotalBytes += (uint)Count;

            if (TotalBytes >= ulong.MaxValue / 8)
                throw new InvalidOperationException("Too much data");
            // Fill existing buffer
            if (bytesInBuffer != 0)
            {
                var toCopy = Math.Min(BlockSize - bytesInBuffer, Count);
                System.Buffer.BlockCopy(Data, Offset, Buffer, bytesInBuffer, toCopy);
                Offset += toCopy;
                Count -= toCopy;
                bytesInBuffer += toCopy;
                if (bytesInBuffer == BlockSize)
                {
                    ByteIntegerConverter.Array16LoadBigEndian64(out block, Buffer, 0);
                    Sha512Internal.Core(out State, ref State, ref block);
                    CryptoBytes.InternalWipe(Buffer, 0, Buffer.Length);
                    bytesInBuffer = 0;
                }
            }
            // Hash complete blocks without copying
            while (Count >= BlockSize)
            {
                ByteIntegerConverter.Array16LoadBigEndian64(out block, Data, Offset);
                Sha512Internal.Core(out State, ref State, ref block);
                Offset += BlockSize;
                Count -= BlockSize;
            }
            // Copy remainder into buffer
            if (Count > 0)
            {
                System.Buffer.BlockCopy(Data, Offset, Buffer, bytesInBuffer, Count);
            }
        }

        public void Finish(ArraySegment<byte> Output)
        {
            if (Output.Array == null)
                throw new ArgumentNullException("output.Array");
            if (Output.Count != 64)
                throw new ArgumentException("output.Count must be 64");

            Update(Padding, 0, Padding.Length);
            Array16<ulong> block;
            ByteIntegerConverter.Array16LoadBigEndian64(out block, Buffer, 0);
            CryptoBytes.InternalWipe(Buffer, 0, Buffer.Length);
            var bytesInBuffer = (int)TotalBytes & (BlockSize - 1);
            if (bytesInBuffer > BlockSize - 16)
            {
                Sha512Internal.Core(out State, ref State, ref block);
                block = default(Array16<ulong>);
            }
            block.x15 = (TotalBytes - 1) * 8;
            Sha512Internal.Core(out State, ref State, ref block);

            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 0, State.x0);
            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 8, State.x1);
            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 16, State.x2);
            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 24, State.x3);
            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 32, State.x4);
            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 40, State.x5);
            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 48, State.x6);
            ByteIntegerConverter.StoreBigEndian64(Output.Array, Output.Offset + 56, State.x7);
            State = default(Array8<ulong>);
        }

        public byte[] Finish()
        {
            var result = new byte[64];
            Finish(new ArraySegment<byte>(result));
            return result;
        }

        public static byte[] Hash(byte[] Data)
        {
            return Hash(Data, 0, Data.Length);
        }

        public static byte[] Hash(byte[] Data, int Offset, int Count)
        {
            var hasher = new Sha512();
            hasher.Update(Data, Offset, Count);
            return hasher.Finish();
        }
    }
}

using System;

namespace Texnomic.Chaos.NaCl.Internal
{
    // Loops? Arrays? Never heard of that stuff
    // Library avoids unnecessary heap allocations and unsafe code
    // so this ugly code becomes necessary :(
    internal static class ByteIntegerConverter
    {
        public static UInt32 LoadLittleEndian32(byte[] Buf, int Offset)
        {
            return
                (UInt32)(Buf[Offset + 0])
            | (((UInt32)(Buf[Offset + 1])) << 8)
            | (((UInt32)(Buf[Offset + 2])) << 16)
            | (((UInt32)(Buf[Offset + 3])) << 24);
        }

        public static void StoreLittleEndian32(byte[] Buf, int Offset, UInt32 Value)
        {
            Buf[Offset + 0] = unchecked((byte)Value);
            Buf[Offset + 1] = unchecked((byte)(Value >> 8));
            Buf[Offset + 2] = unchecked((byte)(Value >> 16));
            Buf[Offset + 3] = unchecked((byte)(Value >> 24));
        }

        public static UInt64 LoadBigEndian64(byte[] Buf, int Offset)
        {
            return
                (UInt64)(Buf[Offset + 7])
                | (((UInt64)(Buf[Offset + 6])) << 8)
                | (((UInt64)(Buf[Offset + 5])) << 16)
                | (((UInt64)(Buf[Offset + 4])) << 24)
                | (((UInt64)(Buf[Offset + 3])) << 32)
                | (((UInt64)(Buf[Offset + 2])) << 40)
                | (((UInt64)(Buf[Offset + 1])) << 48)
                | (((UInt64)(Buf[Offset + 0])) << 56);
        }

        public static void StoreBigEndian64(byte[] Buf, int Offset, UInt64 Value)
        {
            Buf[Offset + 7] = unchecked((byte) Value);
            Buf[Offset + 6] = unchecked((byte) (Value >> 8));
            Buf[Offset + 5] = unchecked((byte) (Value >> 16));
            Buf[Offset + 4] = unchecked((byte) (Value >> 24));
            Buf[Offset + 3] = unchecked((byte) (Value >> 32));
            Buf[Offset + 2] = unchecked((byte) (Value >> 40));
            Buf[Offset + 1] = unchecked((byte) (Value >> 48));
            Buf[Offset + 0] = unchecked((byte) (Value >> 56));
        }

        public static void Array8LoadLittleEndian32(out Array8<UInt32> Output, byte[] Input, int InputOffset)
        {
            Output.x0 = LoadLittleEndian32(Input, InputOffset + 0);
            Output.x1 = LoadLittleEndian32(Input, InputOffset + 4);
            Output.x2 = LoadLittleEndian32(Input, InputOffset + 8);
            Output.x3 = LoadLittleEndian32(Input, InputOffset + 12);
            Output.x4 = LoadLittleEndian32(Input, InputOffset + 16);
            Output.x5 = LoadLittleEndian32(Input, InputOffset + 20);
            Output.x6 = LoadLittleEndian32(Input, InputOffset + 24);
            Output.x7 = LoadLittleEndian32(Input, InputOffset + 28);
        }

        public static void Array16LoadBigEndian64(out Array16<UInt64> Output, byte[] Input, int InputOffset)
        {
            Output.x0 = LoadBigEndian64(Input, InputOffset + 0);
            Output.x1 = LoadBigEndian64(Input, InputOffset + 8);
            Output.x2 = LoadBigEndian64(Input, InputOffset + 16);
            Output.x3 = LoadBigEndian64(Input, InputOffset + 24);
            Output.x4 = LoadBigEndian64(Input, InputOffset + 32);
            Output.x5 = LoadBigEndian64(Input, InputOffset + 40);
            Output.x6 = LoadBigEndian64(Input, InputOffset + 48);
            Output.x7 = LoadBigEndian64(Input, InputOffset + 56);
            Output.x8 = LoadBigEndian64(Input, InputOffset + 64);
            Output.x9 = LoadBigEndian64(Input, InputOffset + 72);
            Output.x10 = LoadBigEndian64(Input, InputOffset + 80);
            Output.x11 = LoadBigEndian64(Input, InputOffset + 88);
            Output.x12 = LoadBigEndian64(Input, InputOffset + 96);
            Output.x13 = LoadBigEndian64(Input, InputOffset + 104);
            Output.x14 = LoadBigEndian64(Input, InputOffset + 112);
            Output.x15 = LoadBigEndian64(Input, InputOffset + 120);
        }

        public static void Array16StoreLittleEndian32(byte[] Output, int OutputOffset, ref Array16<UInt32> Input)
        {
            StoreLittleEndian32(Output, OutputOffset + 0, Input.x0);
            StoreLittleEndian32(Output, OutputOffset + 4, Input.x1);
            StoreLittleEndian32(Output, OutputOffset + 8, Input.x2);
            StoreLittleEndian32(Output, OutputOffset + 12, Input.x3);
            StoreLittleEndian32(Output, OutputOffset + 16, Input.x4);
            StoreLittleEndian32(Output, OutputOffset + 20, Input.x5);
            StoreLittleEndian32(Output, OutputOffset + 24, Input.x6);
            StoreLittleEndian32(Output, OutputOffset + 28, Input.x7);
            StoreLittleEndian32(Output, OutputOffset + 32, Input.x8);
            StoreLittleEndian32(Output, OutputOffset + 36, Input.x9);
            StoreLittleEndian32(Output, OutputOffset + 40, Input.x10);
            StoreLittleEndian32(Output, OutputOffset + 44, Input.x11);
            StoreLittleEndian32(Output, OutputOffset + 48, Input.x12);
            StoreLittleEndian32(Output, OutputOffset + 52, Input.x13);
            StoreLittleEndian32(Output, OutputOffset + 56, Input.x14);
            StoreLittleEndian32(Output, OutputOffset + 60, Input.x15);
        }
    }
}

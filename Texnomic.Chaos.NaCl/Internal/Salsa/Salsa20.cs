using System;

namespace Texnomic.Chaos.NaCl.Internal.Salsa
{
    internal class Salsa20
    {
        public const uint SalsaConst0 = 0x61707865;
        public const uint SalsaConst1 = 0x3320646e;
        public const uint SalsaConst2 = 0x79622d32;
        public const uint SalsaConst3 = 0x6b206574;

        public static void HSalsa20(byte[] Output, int OutputOffset, byte[] Key, int KeyOffset, byte[] Nonce, int NonceOffset)
        {
            Array16<UInt32> state;
            state.x0 = SalsaConst0;
            state.x1 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 0);
            state.x2 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 4);
            state.x3 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 8);
            state.x4 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 12);
            state.x5 = SalsaConst1;
            state.x6 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 0);
            state.x7 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 4);
            state.x8 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 8);
            state.x9 = ByteIntegerConverter.LoadLittleEndian32(Nonce, NonceOffset + 12);
            state.x10 = SalsaConst2;
            state.x11 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 16);
            state.x12 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 20);
            state.x13 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 24);
            state.x14 = ByteIntegerConverter.LoadLittleEndian32(Key, KeyOffset + 28);
            state.x15 = SalsaConst3;

            SalsaCore.HSalsa(out state, ref state, 20);

            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 0, state.x0);
            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 4, state.x5);
            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 8, state.x10);
            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 12, state.x15);
            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 16, state.x6);
            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 20, state.x7);
            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 24, state.x8);
            ByteIntegerConverter.StoreLittleEndian32(Output, OutputOffset + 28, state.x9);
        }
    }
}

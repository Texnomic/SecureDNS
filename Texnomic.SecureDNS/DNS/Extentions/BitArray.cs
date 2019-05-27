namespace System.Collections
{
    public static class BitArrayExtentions
    {
        public static int GetInt(this BitArray BitArray, int Index)
        {
            return BitArray.Get(Index) ? 1 : 0;
        }

        public static BitArray Slice(this BitArray BitArray, int Index, int Length)
        {
            var Slice = new BitArray(Length);

            for (int i = 0; i < Length; i++)
            {
                Slice[i] = BitArray[Index + i];
            }

            return Slice;
        }

        public static byte GetByte(this BitArray BitArray, int Index)
        {
            var Bytes = new byte[1];

            BitArray.Slice(Index, 8).CopyTo(Bytes, 0);

            return Bytes[0];
        }

        public static byte[] GetBytes(this BitArray BitArray, int Index, int Length)
        {
            var Bytes = new byte[Length];

            for (int i = 0; i < Length; i++)
            {
                Bytes[i] = BitArray.GetByte(Index * 8);
            }

            return Bytes;
        }

        public static ushort GetUInt16(this BitArray BitArray, int Index)
        {
            var UShort = new ushort[1];

            BitArray.Slice(Index, 16).CopyTo(UShort, 0);

            return UShort[0];
        }
    }
}

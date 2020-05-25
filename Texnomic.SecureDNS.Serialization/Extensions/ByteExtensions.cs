using System;

namespace Texnomic.SecureDNS.Serialization.Extensions
{
    public static class ByteExtensions
    {
        public static byte GetBits(this byte Byte, byte Index, byte Length)
        {
            if (Index > 7) throw new ArgumentOutOfRangeException(nameof(Index));

            if (Length > 8) throw new ArgumentOutOfRangeException(nameof(Length));

            var Shift = 8 - (Index + Length);

            var Mask = (byte)(0b11111111 >> Index >> Shift << Shift);

            return (byte)((Byte & Mask) >> Shift);
        }

        public static byte SetBits(this byte Byte, byte Index, byte Length, byte Value)
        {
            Value = (byte)(Value << (8 - (Index + Length)));

            for (var I = Index; I <= Index + Length - 1; I++)
            {
                var Bit = Value.GetBit(I);

                Byte = Byte.SetBit(I, Bit);
            }

            return Byte;
        }

        public static byte GetBit(this byte Byte, byte Index)
        {
            return Byte.GetBits(Index, 1);
        }

        public static byte SetBit(this byte Byte, byte Index, byte Value)
        {
            if (Value > 1) throw new ArgumentOutOfRangeException(nameof(Value));

            var Mask = (byte)(0b10000000 >> Index);

            Byte = (byte)(Value == 1 ? Byte | Mask : Byte & (byte)~Mask);

            return Byte;
        }

        public static byte AsByte(this bool Bool)
        {
            return Convert.ToByte(Bool);
        }

        public static bool AsBool(this byte Byte)
        {
            return Byte == 1;
        }

        public static TEnum AsEnum<TEnum>(this byte Byte) where TEnum : Enum
        {
            return (TEnum)(object)Byte;
        }
    }
}

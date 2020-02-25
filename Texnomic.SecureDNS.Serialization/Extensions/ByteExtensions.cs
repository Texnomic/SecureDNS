using System;

namespace Texnomic.SecureDNS.Serialization.Extensions
{
    public static class ByteExtensions
    {
        public static byte GetBits(this byte Byte, byte Index, byte Length)
        {
            if (Index > 7) throw new ArgumentOutOfRangeException(nameof(Index));

            if (Length > 8) throw new ArgumentOutOfRangeException(nameof(Length));

            var Mask = (byte)((0b11111111 >> Index >> 8 - (Index + Length)) << 8 - (Index + Length));

            return (byte)((Byte & Mask) >> 8 - (Index + Length));
        }

        public static void SetBits(this byte Byte, byte Index, byte Length, byte Value)
        {
            Value = (byte)(Value << Length - Index);

            for (int I = Index; I < Length; I++)
            {
                var Bit = Value.GetBit(Index);

                Byte.SetBit(Index, Bit);
            }
        }

        public static byte GetBit(this byte Byte, byte Index)
        {
            return Byte.GetBits(Index, 1);
        }

        public static void SetBit(this byte Byte, byte Index, byte Value)
        {
            if (Value > 1) throw new ArgumentOutOfRangeException(nameof(Value));

            var Mask = (byte)(0b10000000 >> Index);

            Byte = Value == 1 ? Byte |= Mask : Byte &= (byte)~Mask;
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

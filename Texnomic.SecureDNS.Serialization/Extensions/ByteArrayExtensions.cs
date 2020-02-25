using System;
using System.Buffers.Binary;
using System.Text;

namespace Texnomic.SecureDNS.Serialization.Extensions
{
    public static class ByteArrayExtensions
    {
        public static short GetShort(this byte[] Raw, ushort Index)
        {
            return BinaryPrimitives.ReadInt16BigEndian(Raw[Index..(Index + 2)]);
        }

        public static byte[] SetShort(this byte[] Raw, ushort Index, short Value)
        {
            var Bytes = new byte[2];

            BinaryPrimitives.WriteInt16BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

            return Raw;
        }

        public static ushort GetUShort(this byte[] Raw, ushort Index)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(Raw[Index..(Index + 2)]);
        }

        public static byte[] SetUShort(this byte[] Raw, ushort Index, ushort Value)
        {
            var Bytes = new byte[2];

            BinaryPrimitives.WriteUInt16BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

            return Raw;
        }

        public static int GetInt(this byte[] Raw, ushort Index)
        {
            return BinaryPrimitives.ReadInt32BigEndian(Raw[Index..(Index + 4)]);
        }

        public static byte[] SetInt(this byte[] Raw, ushort Index, int Value)
        {
            var Bytes = new byte[4];

            BinaryPrimitives.WriteInt32BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

            return Raw;
        }

        public static uint GetUInt(this byte[] Raw, ushort Index)
        {
            return BinaryPrimitives.ReadUInt32BigEndian(Raw[Index..(Index + 4)]);
        }

        public static byte[] SetUInt(this byte[] Raw, ushort Index, uint Value)
        {
            var Bytes = new byte[4];

            BinaryPrimitives.WriteUInt32BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

            return Raw;
        }

        public static long GetLong(this byte[] Raw, ushort Index)
        {
            return BinaryPrimitives.ReadInt64BigEndian(Raw[Index..(Index + 8)]);
        }

        public static byte[] SetLong(this byte[] Raw, ushort Index, long Value)
        {
            var Bytes = new byte[8];

            BinaryPrimitives.WriteInt64BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

            return Raw;
        }

        public static ulong GetULong(this byte[] Raw, ushort Index)
        {
            return BinaryPrimitives.ReadUInt64BigEndian(Raw[Index..(Index + 8)]);
        }

        public static byte[] SetULong(this byte[] Raw, ushort Index, ulong Value)
        {
            var Bytes = new byte[8];

            BinaryPrimitives.WriteUInt64BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

            return Raw;
        }

        public static TEnum GetEnum<TEnum>(this byte[] Raw, ushort Index) where TEnum : Enum
        {
            var EnumType = typeof(TEnum);

            if (!EnumType.IsEnum) throw new ArgumentException("TEnum Must Be Enumerated Type.");

            if (EnumType == typeof(byte)) return (TEnum)(object)Raw[Index];

            if (EnumType == typeof(ushort)) return (TEnum)(object)Raw[Index..(Index + 2)];

            throw new ArgumentOutOfRangeException(nameof(TEnum));
        }

        public static byte[] SetEnum<TEnum>(this byte[] Raw, ushort Index, TEnum Value) where TEnum : Enum
        {
            var EnumType = typeof(TEnum);

            if (!EnumType.IsEnum) throw new ArgumentException("TEnum Must Be Enumerated Type.");

            if (EnumType == typeof(byte))
            {
                Raw[Index] = (byte)(object)Value;

                return Raw;
            }

            if (EnumType == typeof(ushort))
            {
                var Bytes = BitConverter.GetBytes((ushort)(object)Value);

                Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

                return Raw;
            }

            throw new ArgumentOutOfRangeException(nameof(TEnum));
        }

        public static string GetString(this byte[] Raw, ushort Index, ushort Length)
        {
            return Encoding.ASCII.GetString(Raw[Index..(Index + Length)]);
        }

        public static byte[] SetString(this byte[] Raw, byte Index, string Value)
        {
            var Bytes = Encoding.ASCII.GetBytes(Value);

            Array.Copy(Bytes, 0, Raw, Index, Bytes.Length);

            return Raw;
        }

        public static byte[] TrimEnd(this byte[] Raw, byte Byte = 0)
        {
            for (var I = Raw.Length - 1; I > -1; I--)
            {
                if (Raw[I] != Byte) return Raw[..++I];
            }

            return Raw;
        }

        public static byte[] Concat(this byte[] FirstArray, byte[] SecondArray)
        {
            var ConcatArray = new byte[FirstArray.Length + SecondArray.Length];

            Array.Copy(FirstArray, ConcatArray, FirstArray.Length);

            Array.Copy(SecondArray, 0, ConcatArray, FirstArray.Length - 1, SecondArray.Length);

            return ConcatArray;
        }
    }

}

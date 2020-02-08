using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitStreams
{
    /// <summary>
    /// Represents a 48-bit signed integer
    /// </summary>
    [Serializable]
    public struct Int48
    {
        private byte b0, b1, b2, b3, b4, b5;
        private Bit sign;

        private Int48(long value)
        {
            this.b0 = (byte)(value & 0xFF);
            this.b1 = (byte)((value >> 8) & 0xFF);
            this.b2 = (byte)((value >> 16) & 0xFF);
            this.b3 = (byte)((value >> 24) & 0xFF);
            this.b4 = (byte)((value >> 32) & 0xFF);
            this.b5 = (byte)((value >> 40) & 0x7F);
            this.sign = (byte)((value >> 47) & 1);
        }

        public static implicit operator Int48(long value)
        {
            return new Int48(value);
        }

        public static implicit operator long (Int48 i)
        {
            long value = i.b0 + (i.b1 << 8) + (i.b2 << 16) + ((long)i.b3 << 24) + ((long)i.b4 << 32) + ((long)i.b5 << 40);
            return -((long)i.sign << 47) + value;
        }

        public Bit GetBit(int index)
        {
            return (byte)(this >> index);
        }
    }

    /// <summary>
    /// Represents a 48-bit unsigned integer
    /// </summary>
    [Serializable]
    public struct UInt48
    {
        private byte b0, b1, b2, b3, b4, b5;

        private UInt48(ulong value)
        {
            this.b0 = (byte)(value & 0xFF);
            this.b1 = (byte)((value >> 8) & 0xFF);
            this.b2 = (byte)((value >> 16) & 0xFF);
            this.b3 = (byte)((value >> 24) & 0xFF);
            this.b4 = (byte)((value >> 32) & 0xFF);
            this.b5 = (byte)((value >> 40) & 0xFF);
        }

        public static implicit operator UInt48(ulong value)
        {
            return new UInt48(value);
        }

        public static implicit operator ulong (UInt48 i)
        {
            ulong value = (i.b0 + ((ulong)i.b1 << 8) + ((ulong)i.b2 << 16) + ((ulong)i.b3 << 24) + ((ulong)i.b4 << 32) + ((ulong)i.b5 << 40));
            return value;
        }

        public Bit GetBit(int index)
        {
            return (byte)(this >> index);
        }
    }
}
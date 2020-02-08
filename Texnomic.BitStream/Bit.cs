using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitStreams
{
    [Serializable]
    public struct Bit
    {
        private byte value;

        private Bit(int value)
        {
            this.value = (byte)(value & 1);
        }

        public static implicit operator Bit(int value)
        {
            return new Bit(value);
        }

        public static implicit operator Bit(bool value)
        {
            return new Bit(value ? 1 : 0);
        }

        public static implicit operator int (Bit bit)
        {
            return bit.value;
        }

        public static implicit operator byte (Bit bit)
        {
            return (byte)bit.value;
        }

        public static implicit operator bool (Bit bit)
        {
            return bit.value == 1;
        }

        public static Bit operator &(Bit x, Bit y)
        {
            return x.value & y.value;
        }

        public static Bit operator |(Bit x, Bit y)
        {
            return x.value | y.value;
        }

        public static Bit operator ^(Bit x, Bit y)
        {
            return x.value ^ y.value;
        }

        public static Bit operator ~(Bit bit)
        {
            return (~(bit.value) & 1);
        }

        public static implicit operator string (Bit bit)
        {
            return bit.value.ToString();
        }

        public int AsInt()
        {
            return this.value;
        }

        public bool AsBool()
        {
            return this.value == 1;
        }
    }
}

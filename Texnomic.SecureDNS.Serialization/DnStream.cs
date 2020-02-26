using System;
using System.Text;
using System.Buffers.Binary;
using System.Net;
using Texnomic.SecureDNS.Serialization.Extensions;

namespace Texnomic.SecureDNS.Serialization
{
    public class DnStream
    {
        private byte[] Raw;

        private ushort ByteIndex;

        private byte BitIndex;

        public ushort BytePosition => ByteIndex;

        public ushort BitPosition => BitIndex;

        public DnStream()
        {
            Raw = new byte[12];
        }

        public DnStream(ref byte[] Raw)
        {
            this.Raw = Raw;

            (ByteIndex, BitIndex) = (0, 0);
        }

        public void Seek(ushort Bytes, byte Bits = 0)
        {
            (ByteIndex, BitIndex) = (Bytes, Bits);
        }

        public byte GetBits(byte Length)
        {
            var Byte = Raw[ByteIndex].GetBits(BitIndex, Length);

            BitIndex += Length;

            if (BitIndex >= 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }

            return Byte;
        }

        public void SetBits(byte Length, byte Value)
        {
            BitIndex++;

            if (BitIndex == 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }

            Raw[ByteIndex].SetBits(BitIndex, Length, Value);
        }

        public byte GetBit()
        {
            var Byte = Raw[ByteIndex].GetBit(BitIndex);

            BitIndex++;

            if (BitIndex == 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }

            return Byte;
        }

        public void SetBit(byte Value)
        {
            BitIndex++;

            if (BitIndex == 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }

            Raw[ByteIndex].SetBit(BitIndex, Value);
        }

        public byte GetByte()
        {
            var Byte = Raw[ByteIndex];

            ByteIndex += 1;

            return Byte;
        }

        public byte[] GetBytes(ushort Length)
        {
            var Bytes = Raw[ByteIndex..(ByteIndex + Length)];

            ByteIndex += Length;

            return Bytes;
        }

        public short GetShort()
        {
            return BinaryPrimitives.ReadInt16BigEndian(GetBytes(2));
        }

        public void SetShort(short Value)
        {
            ByteIndex++;

            var Bytes = new byte[2];

            BinaryPrimitives.WriteInt16BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public ushort GetUShort()
        {
            return BinaryPrimitives.ReadUInt16BigEndian(GetBytes(2));
        }

        public void SetUShort(ushort Value)
        {
            ByteIndex++;

            var Bytes = new byte[2];

            BinaryPrimitives.WriteUInt16BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public int GetInt()
        {
            return BinaryPrimitives.ReadInt32BigEndian(GetBytes(4));
        }

        public void SetInt(int Value)
        {
            ByteIndex++;

            var Bytes = new byte[4];

            BinaryPrimitives.WriteInt32BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public uint GetUInt()
        {
            return BinaryPrimitives.ReadUInt32BigEndian(GetBytes(4));
        }

        public void SetUInt(uint Value)
        {
            ByteIndex++;

            var Bytes = new byte[4];

            BinaryPrimitives.WriteUInt32BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public long GetLong()
        {
            return BinaryPrimitives.ReadInt64BigEndian(GetBytes(8));
        }

        public void SetLong(long Value)
        {
            ByteIndex++;

            var Bytes = new byte[8];

            BinaryPrimitives.WriteInt64BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public ulong GetULong()
        {
            return BinaryPrimitives.ReadUInt64BigEndian(GetBytes(8));
        }

        public void SetULong(ulong Value)
        {
            ByteIndex++;

            var Bytes = new byte[8];

            BinaryPrimitives.WriteUInt64BigEndian(Bytes, Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public string GetString(ushort Length)
        {
            return Encoding.ASCII.GetString(GetBytes(Length));
        }

        public void SetString(string Value)
        {
            ByteIndex++;

            var Bytes = Encoding.ASCII.GetBytes(Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public TimeSpan GetTimeSpan()
        {
            var Seconds = GetInt();

            return new TimeSpan(0, 0, Seconds);
        }

        public IPAddress GetIPv4Address()
        {
            var Bytes = GetBytes(4);

            return new IPAddress(Bytes);
        }

        public IPAddress GetIPv6Address()
        {
            var Bytes = GetBytes(16);

            return new IPAddress(Bytes);
        }
    }
}

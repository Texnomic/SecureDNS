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

        private int ByteIndex;

        private int BitIndex;

        public ushort BytePosition => (ushort)ByteIndex;

        public ushort BitPosition => (ushort)BitIndex;

        public DnStream()
        {
            Raw = new byte[512];

            (ByteIndex, BitIndex) = (0, 0);
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
            var Byte = Raw[ByteIndex].GetBits((byte)BitIndex, Length);

            BitIndex += Length;

            if (BitIndex > 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }

            return Byte;
        }

        public void SetBits(byte Length, byte Value)
        {
            Raw[ByteIndex] = Raw[ByteIndex].SetBits((byte)BitIndex, Length, Value);

            BitIndex += Length;

            if (BitIndex > 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }
        }

        public byte GetBit()
        {
            var Byte = Raw[ByteIndex].GetBit((byte)BitIndex);

            BitIndex++;

            if (BitIndex > 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }

            return Byte;
        }

        public void SetBit(byte Value)
        {
            Raw[ByteIndex] = Raw[ByteIndex].SetBit((byte)BitIndex, Value);

            BitIndex++;

            if (BitIndex > 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }
        }

        public byte GetByte()
        {
            var Byte = Raw[ByteIndex];

            ByteIndex += 1;

            return Byte;
        }

        public void SetByte(byte Value)
        {
            Raw[ByteIndex] = Value;

            ByteIndex++;
        }

        public byte[] GetBytes(ushort Length)
        {
            var Bytes = Raw[ByteIndex..(ByteIndex + Length)];

            ByteIndex += Length;

            return Bytes;
        }

        public void SetBytes(byte[] Bytes)
        {
            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort) Bytes.Length;
        }

        public short GetShort()
        {
            return BinaryPrimitives.ReadInt16BigEndian(GetBytes(2));
        }

        public void SetShort(short Value)
        {
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
            var Bytes = Encoding.ASCII.GetBytes(Value);

            Array.Copy(Bytes, 0, Raw, ByteIndex, Bytes.Length);

            ByteIndex += (ushort)Bytes.Length;
        }

        public TimeSpan GetTimeSpan()
        {
            var Seconds = GetInt();

            return new TimeSpan(0, 0, Seconds);
        }

        public void SetTimeSpan(TimeSpan TimeSpan)
        {
            SetUInt((uint)TimeSpan.TotalSeconds);
        }

        public IPAddress GetIPv4Address()
        {
            var Bytes = GetBytes(4);

            return new IPAddress(Bytes);
        }

        public void SetIPv4Address(IPAddress IPAddress)
        {
            var Bytes = IPAddress.GetAddressBytes();

            //Array.Reverse(Bytes);

            SetBytes(Bytes);
        }

        public IPAddress GetIPv6Address()
        {
            var Bytes = GetBytes(16);

            return new IPAddress(Bytes);
        }

        public void SetIPv6Address(IPAddress IPAddress)
        {
            var Bytes = IPAddress.GetAddressBytes();

            //Array.Reverse(Bytes);

            SetBytes(Bytes);
        }

        public byte[] ToArray()
        {
            return Raw.TrimEnd();
        }
    }
}

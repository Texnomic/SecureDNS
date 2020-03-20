using System;
using System.Text;
using System.Buffers.Binary;
using System.Net;
using Texnomic.SecureDNS.Serialization.Extensions;

namespace Texnomic.SecureDNS.Serialization
{
    public class DnStream
    {
        private readonly Memory<byte> Raw;

        private int ByteIndex;

        private int BitIndex;

        public ushort BytePosition => (ushort)ByteIndex;

        public ushort BitPosition => (ushort)BitIndex;


        public DnStream(ushort Length)
        {
            Raw = new byte[Length];

            (ByteIndex, BitIndex) = (0, 0);
        }

        public DnStream(in byte[] Raw)
        {
            this.Raw = Raw;

            (ByteIndex, BitIndex) = (0, 0);
        }

        public void Seek(ushort Bytes, byte Bits = 0)
        {
            (ByteIndex, BitIndex) = (Bytes, Bits);
        }


        private Span<byte> GetSpan(ushort Length)
        {
            return Raw.Slice(ByteIndex, Length).Span;
        }

        private Span<byte> GetSpan(int Length)
        {
            return Raw.Slice(ByteIndex, Length).Span;
        }

        public byte GetBits(byte Length)
        {
            var Byte = Raw.Span[ByteIndex].GetBits((byte)BitIndex, Length);

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
            Raw.Span[ByteIndex] = Raw.Span[ByteIndex].SetBits((byte)BitIndex, Length, Value);

            BitIndex += Length;

            if (BitIndex > 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }
        }

        public byte GetBit()
        {
            var Byte = Raw.Span[ByteIndex].GetBit((byte)BitIndex);

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
            Raw.Span[ByteIndex] = Raw.Span[ByteIndex].SetBit((byte)BitIndex, Value);

            BitIndex++;

            if (BitIndex > 7)
            {
                ByteIndex++;
                BitIndex = 0;
            }
        }

        public byte GetByte()
        {
            var Byte = Raw.Span[ByteIndex];

            ByteIndex += 1;

            return Byte;
        }

        public void SetByte(byte Value)
        {
            Raw.Span[ByteIndex] = Value;

            ByteIndex++;
        }

        public ReadOnlySpan<byte> ReadBytes(ushort Length)
        {
            var Bytes = GetSpan(Length);

            ByteIndex += Length;

            return Bytes;
        }

        public void WriteBytes(Span<byte> Bytes)
        {
            Bytes.CopyTo(GetSpan(Bytes.Length));

            ByteIndex += Bytes.Length;
        }

        public void WriteBytes(ReadOnlySpan<byte> Bytes)
        {
            Bytes.CopyTo(GetSpan(Bytes.Length));

            ByteIndex += Bytes.Length;
        }

        public short ReadShort()
        {
            return BinaryPrimitives.ReadInt16BigEndian(ReadBytes(2));
        }

        public void WriteShort(short Value)
        {
            BinaryPrimitives.WriteInt16BigEndian(GetSpan(2), Value);

            ByteIndex += 2;
        }

        public ushort ReadUShort()
        {
            return BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(2));
        }

        public void WriteUShort(ushort Value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(GetSpan(2), Value);

            ByteIndex += 2;
        }

        public int ReadInt32()
        {
            return BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));
        }

        public void WriteInt32(int Value)
        {
            BinaryPrimitives.WriteInt32BigEndian(GetSpan(4), Value);

            ByteIndex += 4;
        }

        public uint ReadUInt32()
        {
            return BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(4));
        }

        public void WriteUInt32(uint Value)
        {
            BinaryPrimitives.WriteUInt32BigEndian(GetSpan(4), Value);

            ByteIndex += 4;
        }

        public long ReadLong()
        {
            return BinaryPrimitives.ReadInt64BigEndian(ReadBytes(8));
        }

        public void WriteLong(long Value)
        {
            BinaryPrimitives.WriteInt64BigEndian(GetSpan(8), Value);

            ByteIndex += 8;
        }

        public ulong ReadULong()
        {
            return BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(8));
        }

        public void WriteULong(ulong Value)
        {
            BinaryPrimitives.WriteUInt64BigEndian(GetSpan(8), Value);

            ByteIndex += 8;
        }

        public string ReadString(ushort Length)
        {
            return Encoding.ASCII.GetString(ReadBytes(Length));
        }

        public void WriteString(string Value)
        {
            var Bytes = Encoding.ASCII.GetBytes(Value);

            Bytes.CopyTo(GetSpan(Bytes.Length));

            ByteIndex += Bytes.Length;
        }

        public TimeSpan ReadTimeSpan()
        {
            var Seconds = ReadInt32();

            return new TimeSpan(0, 0, Seconds);
        }

        public void WriteTimeSpan(TimeSpan TimeSpan)
        {
            WriteUInt32((uint)TimeSpan.TotalSeconds);
        }

        public IPAddress ReadIPv4Address()
        {
            return new IPAddress(ReadBytes(4));
        }

        public void WriteIPv4Address(IPAddress IPAddress)
        {
            WriteBytes(IPAddress.GetAddressBytes());
        }

        public IPAddress ReadIPv6Address()
        {
            return new IPAddress(ReadBytes(16));
        }

        public void WriteIPv6Address(IPAddress IPAddress)
        {
            WriteBytes(IPAddress.GetAddressBytes());
        }

        public DateTime ReadEpoch()
        {
            var Seconds = ReadUInt32();

            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Seconds);
        }

        public void WriteEpoch(DateTime DateTime)
        {
            var Seconds = DateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            WriteUInt32((uint)Seconds);
        }

        public byte[] ToArray()
        {
            return Raw.ToArray();
        }
    }
}

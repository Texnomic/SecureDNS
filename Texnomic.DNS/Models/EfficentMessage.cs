using System;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Models
{
    public class EfficientMessage
    {
        private readonly Memory<byte> Raw;

        public EfficientMessage()
        {
            Raw = new Memory<byte>(new byte[12]);
        }

        public EfficientMessage(byte[] Raw)
        {
            this.Raw = new Memory<byte>(Raw);
        }

        public EfficientMessage(ReadOnlySpan<byte> Raw)
        {
            this.Raw = new Memory<byte>(Raw.ToArray());
        }

        public ushort ID
        {
            get => GetID();
            set => SetID(value);
        }

        public MessageType MessageType
        {
            get => GetMessageType();
            set => SetMessageType(value);
        }

        private ushort GetID()
        {
            return GetUInt16(0);
        }

        private MessageType GetMessageType()
        {
            return (MessageType)GetBit(2, 1);
        }

        private void SetMessageType(MessageType MessageType)
        {
            SetBit(2, 1, Convert.ToBoolean(MessageType));
        }
        private void SetID(ushort Value)
        {
            SetUInt16(0, Value);
        }
        private ushort GetUInt16(int Offset)
        {
            var Bytes = GetBytes(Offset, 2);
            return BitConverter.ToUInt16(Bytes);
        }
        private void SetUInt16(int Offset, ushort Value)
        {
            var Bytes = BitConverter.GetBytes(Value);
            SetBytes(Offset, Bytes);
        }
        private byte GetBit(int Offset, int Order)
        {
            var Byte = GetByte(Offset);

            return (byte)(Byte & ~(255 >> Order));
        }
        private void SetBit(int Offset, int Order, bool Value)
        {
            var Byte = GetByte(Offset);

            var Mask = (byte)(Byte & (1 << Order));

            if (Value)
            {
                Byte |= Mask;
            }
            else
            {
                Byte &= (byte)(~Mask);
            }

            SetByte(Offset, Byte);
        }
        private byte GetByte(int Offset)
        {
            return (byte)(Raw.Span[Offset] >> 8);
        }

        private void SetByte(int Offset, byte Value)
        {
            Raw.Span[Offset] = (byte)(Value >> 8);
        }
        private Span<byte> GetBytes(int Offset, int Length)
        {
            var Slice = Raw.Span.Slice(Offset, Length);
            Slice.Reverse();
            return Slice;
        }
        private void SetBytes(int Offset, byte[] Bytes)
        {
            Array.Reverse(Bytes);
            Raw.Span[Offset + 0] = Bytes[0];
            Raw.Span[Offset + 1] = Bytes[1];
        }
    }
}

using BinarySerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestSharp.Extensions;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Models
{
    public class Domain : IBinarySerializable, IDomain
    {
        [Ignore]
        public List<ILabel> Labels { get; }

        [Ignore]
        public string Name => ToString();

        /// <summary>
        /// Empty Constructor for Serialization
        /// </summary>
        public Domain()
        {
            Labels = new List<ILabel>();
        }

        public Domain(string Domain)
        {
            Labels = new List<ILabel>();
            Labels.AddRange(Domain.Split('.')
                    .Select(Label => new Label
                    {
                        Type = LabelType.Normal,
                        Count = (ushort)Label.Length,
                        Text = Label
                    }));
        }

        public override string ToString()
        {
            return string.Join('.', Labels.Select(Label => Label.Text));
        }

        public static Domain FromString(string Domain)
        {
            return new Domain(Domain);
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            const byte NullOctet = 0b00000000;

            var Domain = Context.Value as Domain;

            foreach (var Label in Domain.Labels)
            {
                var Flag = (ushort) Label.Type << 6;

                var FlagSize = (byte) (Flag + Label.Count);

                Stream.WriteByte(FlagSize);

                var Bytes = Encoding.ASCII.GetBytes(Label.Text);

                Stream.Write(Bytes, 0, Bytes.Length);
            }

            Stream.WriteByte(NullOctet);
        }


        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            var Root = GetRootStream(Stream);

            var GlobalPosition = Root.Position;

            Root.Position = 0;

            var Bytes = Root.ReadAsBytes();

            Root.Position = GlobalPosition;

            Deserialize(ref Stream, ref Bytes, (int)GlobalPosition);
        }

        private void Deserialize(ref Stream Stream, ref byte[] Bytes, int Offset, bool Reposition = true)
        {
            if (Bytes[Offset] == 0)
            {
                if (Reposition) Stream.Position += 1; 
                return;
            }

            var LabelType = GetLabelType(Bytes[Offset]);

            switch (LabelType)
            {
                case LabelType.Normal:
                    {
                        var LabelLength = GetLabelLength(Bytes[Offset]);

                        Offset += 1;

                        if(Reposition) Stream.Position += LabelLength + 1;

                        GetLabel(ref Bytes, Offset, LabelLength);

                        Deserialize(ref Stream, ref Bytes, Offset + LabelLength, Reposition);

                        break;
                    }
                case LabelType.Compressed:
                    {
                        var Pointer = GetPointer(Bytes[Offset], Bytes[Offset + 1]);

                        Stream.Position += 2;

                        Deserialize(ref Stream, ref Bytes, Pointer, false);

                        break;
                    }
                case LabelType.Extended:
                case LabelType.Unallocated:
                //default: throw new NotImplementedException(Enum.GetName(typeof(LabelType), LabelType));
                default: break;
            }
        }

        private int GetLabelLength(byte Byte)
        {
            const byte LengthMask = 0b11000000;
            return Byte & ~LengthMask;
        }
        private int GetPointer(int FirstByte, int SecondByte)
        {
            const byte PointerMask = 0b11000000;
            return (FirstByte & ~PointerMask) + SecondByte;
        }
        private void GetLabel(ref byte[] Bytes, int Offset, int Length)
        {
            var Buffer = new byte[Length];

            Array.Copy(Bytes, Offset, Buffer, 0, Length);

            var String = Encoding.ASCII.GetString(Buffer);

            var Label = new Label
            {
                Type = LabelType.Normal,
                Text = String,
                Count = (ushort)String.Length,
            };

            Labels.Add(Label);
        }
        private LabelType GetLabelType(byte Byte)
        {
            const byte LabelMask = 0b00111111;

            var Flag = (Byte & ~LabelMask) >> 6;

            return (LabelType)Flag;
        }
        private MemoryStream GetRootStream(Stream Stream)
        {
            var BoundedStream = Stream as BoundedStream;

            while (true)
            {
                if (BoundedStream?.Source is BoundedStream Source)
                {
                    BoundedStream = Source;
                }
                else
                {
                    return BoundedStream?.Source as MemoryStream;
                }
            }
        }
    }
}

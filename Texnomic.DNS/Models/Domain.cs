using BinarySerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Models
{
    public class Domain : IBinarySerializable, IDomain
    {
        [Ignore]
        public List<ILabel> Labels { get; private set; }

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
            SerializeLabels(Stream, Context);
        }


        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            var Root = GetRootStream(Stream);

            var Byte = Root.ReadByte();

            while (Byte > 0)
            {
                Process(Root, Byte);

                Byte = Root.ReadByte();
            }
        }

        private void Process(Stream Stream, int Byte)
        {
            var LabelType = GetLabelType(Byte);

            switch (LabelType)
            {
                case LabelType.Normal:
                    {
                        var LabelLength = GetLabelLength(Byte);

                        GetLabel(Stream, LabelLength);

                        break;
                    }
                case LabelType.Compressed:
                    {
                        var SecondByte = Stream.ReadByte();

                        Stream.Position = GetPointer(Byte, SecondByte);

                        break;
                    }
                case LabelType.Extended:
                case LabelType.Unallocated:
                default: throw new NotImplementedException(Enum.GetName(typeof(Abstractions.Enums.LabelType), LabelType));
            }
        }

        private void SerializeLabels(Stream Stream, BinarySerializationContext Context)
        {
            const byte NullOctet = 0b00000000;

            var Domain = Context.Value as Domain;

            foreach (var Label in Domain.Labels)
            {
                var Flag = (ushort)Label.Type << 6;

                var FlagSize = (byte)(Flag + Label.Count);

                Stream.WriteByte(FlagSize);

                var Bytes = Encoding.ASCII.GetBytes(Label.Text);

                Stream.Write(Bytes, 0, Bytes.Length);
            }

            Stream.WriteByte(NullOctet);
        }



        private int GetLabelLength(int Byte)
        {
            const byte LengthMask = 0b11000000;
            return Byte & ~LengthMask;
        }
        private int GetPointer(int FirstByte, int SecondByte)
        {
            const byte PointerMask = 0b11000000;
            return (FirstByte & ~PointerMask) + SecondByte;
        }
        private void GetLabel(Stream Stream, int Length)
        {
            var Buffer = new byte[Length];

            Stream.Read(Buffer, 0, Length);

            var String = Encoding.ASCII.GetString(Buffer);

            var Label = new Label
            {
                Type = Abstractions.Enums.LabelType.Normal,
                Text = String,
                Count = (ushort)String.Length,
            };

            Labels.Add(Label);
        }
        private Abstractions.Enums.LabelType GetLabelType(int Byte)
        {
            const byte LabelMask = 0b00111111;

            var Flag = (Byte & ~LabelMask) >> 6;

            return (Abstractions.Enums.LabelType)Flag;
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

        IDomain IDomain.FromString(string Domain)
        {
            throw new NotImplementedException();
        }
    }
}

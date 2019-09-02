using BinarySerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Domain : IBinarySerializable
    {
        [Ignore]
        public List<Label> Labels { get; private set; }

        [Ignore]
        public string Name => ToString();

        /// <summary>
        /// Empty Constructor for Serialization
        /// </summary>
        public Domain()
        {
            Labels = new List<Label>();
        }

        public Domain(string Domain)
        {
            Labels = Domain.Split('.')
                           .Select(Label => new Label
                           {
                               Type = LabelType.Normal,
                               Count = (ushort)Label.Length,
                               Text = Label
                           })
                           .ToList();
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
            if (Context.ParentContext.Value is Answer)
            {
                SerializeCompressedLabels(Stream);
            }
            else
            {
                SerializeNormalLabels(Stream, Context);
            }
        }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            var Type = ReadLabelType(Stream);

            switch (Type)
            {
                case LabelType.Normal:
                    {
                        DeserializeNormalLabels(Stream);

                        break;
                    }
                case LabelType.Compressed:
                    {
                        DeserializeCompressedLabels(Stream, Context);

                        break;
                    }
                case LabelType.Extended:
                case LabelType.Unallocated:
                default: throw new NotImplementedException(Enum.GetName(typeof(LabelType), Type));
            }
        }

        private void DeserializeNormalLabels(Stream Stream)
        {
            const byte LengthMask = 0b11000000;

            Stream.Position = 0;

            var Byte = Stream.ReadByte();

            while (Byte != 0)
            {
                var Length = Byte & ~LengthMask;

                var Buffer = new byte[Length];

                Stream.Read(Buffer, 0, Length);

                var String = Encoding.ASCII.GetString(Buffer);

                var Label = new Label
                {
                    Type = LabelType.Normal,
                    Text = String,
                    Count = (ushort)String.Length,
                };

                Labels.Add(Label);

                Byte = Stream.ReadByte();
            }
        }
        private void DeserializeCompressedLabels(Stream Stream, BinarySerializationContext Context)
        {
            const byte PointerMask = 0b11000000;

            Stream.Position = 0;

            var Byte = Stream.ReadByte();

            var Pointer = (Byte & ~PointerMask) + Stream.ReadByte();

            if (Pointer != 12) throw new NotImplementedException($"Compressed Label with OffSet Pointer {Pointer}.");

            var Message = Context.ParentContext.ParentContext.ParentValue as Message;

            Labels = Message.Questions.First().Domain.Labels;
        }

        private void SerializeNormalLabels(Stream Stream, BinarySerializationContext Context)
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
        private void SerializeCompressedLabels(Stream Stream)
        {
            const byte CompressedLabel = 0b11000000;
            const byte LabelPointer = 0b00001100;

            Stream.WriteByte(CompressedLabel);
            Stream.WriteByte(LabelPointer);
        }

        private LabelType ReadLabelType(Stream Stream)
        {
            const byte LabelMask = 0b00111111;

            var Byte = Stream.ReadByte();

            var Flag = (Byte & ~LabelMask) >> 6;

            return (LabelType)Flag;
        }
    }
}

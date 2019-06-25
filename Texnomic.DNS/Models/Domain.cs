using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Domain : IBinarySerializable
    {
        [Ignore]
        public Label[] Labels { get; set; }

        public override string ToString()
        {
            return string.Join('.', Labels.Select(Label => Label.Text));
        }

        public static Domain FromString(string DomainText)
        {
            var DomainLables = DomainText.Split('.');

            var Domain = new Domain
            {
                Labels = new Label[DomainLables.Length]
            };

            for (int i = 0; i < DomainLables.Length; i++)
            {
                Domain.Labels[i] = new Label
                {
                    Type = LabelType.Normal,
                    Count = (ushort)DomainLables[i].Length,
                    Text = DomainLables[i]
                };
            }

            return Domain;
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            if(Context.ParentContext.Value is Answer)
            {
                Stream.WriteByte(0b11000000);
                Stream.WriteByte(0b00001100);
            }
            else
            {
                SerializeLabels(Stream);
            }
        }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            var Data = Stream.ReadByte();

            var Flag = (Data &~ 0b00111111) >> 6;

            var Type = (LabelType)(Flag);

            switch (Type)
            {
                case LabelType.Normal:
                    {
                        Stream.Position = 0;

                        DeserializeLabels(Stream);

                        break;
                    }
                case LabelType.Compressed:
                    {
                        var Byte = Stream.ReadByte();

                        var Pointer = (Byte &~ 0b11000000) + Stream.ReadByte();

                        if (Pointer != 12) throw new NotImplementedException();

                        var Message = Context.ParentContext.ParentContext.ParentValue as Message;

                        Labels = Message.Questions.First().Domain.Labels;

                        Stream.Position -= 1; //Workaround Serializer Bug

                        break;
                    }
                case LabelType.Extended:
                case LabelType.Unallocated:
                default: throw new NotImplementedException(Enum.GetName(typeof(LabelType), Type));
            }
        }

        private void DeserializeLabels(Stream Stream)
        {
            var Byte = Stream.ReadByte();

            var Labels = new List<Label>();

            while (Byte != 0)
            {
                var Size = Byte & ~0b11000000;

                var Buffer = new byte[Size];

                Stream.Read(Buffer, 0, Size);

                var Text = Encoding.ASCII.GetString(Buffer);

                var Label = new Label
                {
                    Type = LabelType.Normal,
                    Text = Text,
                    Count = (ushort)Text.Length,
                };

                Labels.Add(Label);

                Byte = Stream.ReadByte();
            }

            this.Labels = Labels.ToArray();
        }

        private void SerializeLabels(Stream Stream)
        {
            foreach (var Label in Labels)
            {
                var Flag = (ushort)Label.Type << 6;

                var FlagSize = (byte)(Flag + Label.Count);

                Stream.WriteByte(FlagSize);

                var Bytes = Encoding.ASCII.GetBytes(Label.Text);

                Stream.Write(Bytes, 0, Bytes.Length);
            }

            Stream.WriteByte(0);
        }
    }
}

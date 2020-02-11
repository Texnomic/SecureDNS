using BinarySerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;

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

        public static implicit operator string(Domain Domain)
        {
            return Domain?.ToString();
        }

        public static implicit operator Domain(string Domain)
        {
            return new Domain(Domain);
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
                var Flag = (ushort)Label.Type << 6;

                var FlagSize = (byte)(Flag + Label.Count);

                Stream.WriteByte(FlagSize);

                var Bytes = Encoding.ASCII.GetBytes(Label.Text);

                Stream.Write(Bytes, 0, Bytes.Length);
            }

            Stream.WriteByte(NullOctet);
        }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            while (true)
            {
                var Byte = (byte)Stream.ReadByte();

                if (Byte == 0) return;

                var LabelType = GetLabelType(Byte);

                switch (LabelType)
                {
                    case LabelType.Normal:
                        {
                            var LabelLength = GetLabelLength(Byte);

                            GetLabel(Stream, LabelLength);

                            continue;
                        }
                    case LabelType.Compressed:
                        {
                            var Pointer = GetPointer(Byte, Stream.ReadByte());

                            var Message = Context.FindAncestor<Message>();

                            var QLabel = Message.Questions
                                                .Select(Question => Question.Domain)
                                                .Where(Domain => Domain.Labels.Any(Label => Label.Offset == Pointer))
                                                .SelectMany(Domain => Domain.Labels)
                                                .Where(Label => Label.Offset >= Pointer);

                            Labels.AddRange(QLabel);

                            return;
                        }
                    case LabelType.Extended:
                    case LabelType.Unallocated:
                        throw new NotImplementedException(Enum.GetName(typeof(LabelType), LabelType));
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LabelType));
                }
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
        private void GetLabel(Stream Stream, int Length)
        {
            var Offset = ((BoundedStream)Stream).GlobalPosition.TotalByteCount - 1;

            var Buffer = new byte[Length];

            Stream.Read(Buffer);

            var String = Encoding.ASCII.GetString(Buffer);

            var Label = new Label
            {
                Type = LabelType.Normal,
                Text = String,
                Offset = Offset,
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


        public byte[] ToArray()
        {
            var Serializer = new BinarySerializer();
            return Serializer.Serialize(this);
        }

        public async Task<byte[]> ToArrayAsync()
        {
            var Serializer = new BinarySerializer();
            return await Serializer.SerializeAsync(this);
        }
    }
}

using BinarySerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitStreams;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Models
{
    [LogAsScalar(true)]
    public class Domain : IBinarySerializable, IDomain
    {
        [Ignore]
        public List<ILabel> Labels { get; private set; }

        [Ignore]
        public string Name => ToString();

        public Domain()
        {
            Labels = new List<ILabel>();
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            const byte NullOctet = 0b00000000;

            var Domain = (Domain)Context.Value;

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
            var BoundedStream = (BoundedStream)Stream;

            var BitStream = new BitStream(BoundedStream, true);

            while (true)
            {
                var LabelType = (LabelType)(BitStream.ReadByte(2) >> 6);

                switch (LabelType)
                {
                    case LabelType.Normal:
                        {
                            var Length = BitStream.ReadByte(6) >> 2; //Compensate for MSB

                            if (Length == 0) return;

                            Labels.Add(new Label
                            {
                                Type = LabelType.Normal,
                                Count = (ushort)Length,
                                Offset = BoundedStream.GlobalPosition.TotalByteCount - 1,
                                Text = BitStream.ReadString(Length),
                            });

                            continue;
                        }
                    case LabelType.Compressed:
                        {
                            var Pointer = BitStream.ReadByte(6) + BitStream.ReadByte();

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

        public static implicit operator string(Domain Domain)
        {
            return Domain?.ToString();
        }

        public static implicit operator Domain(string FQDN)
        {
            return FromString(FQDN);
        }

        public override string ToString()
        {
            return string.Join('.', Labels.Select(Label => Label.Text));
        }

        public static Domain FromString(string FQDN)
        {
            var Domain = new Domain
            {
                Labels = new List<ILabel>()
            };

            Domain.Labels.AddRange(
                FQDN.Split('.')
                    .Select(Label => new Label
                    {
                        Type = LabelType.Normal,
                        Count = (ushort)Label.Length,
                        Text = Label
                    }));

            return Domain;
        }
    }
}

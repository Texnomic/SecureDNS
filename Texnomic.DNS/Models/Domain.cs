using BinarySerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destructurama.Attributed;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Records;
using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Serialization;

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
            foreach (var Label in Labels)
            {
                switch (Label.Type)
                {
                    case LabelType.Normal:
                        {
                            var Flag = (ushort) Label.Type << 6;

                            var FlagSize = (byte) (Flag + Label.Count);

                            Stream.WriteByte(FlagSize);

                            var Bytes = Encoding.ASCII.GetBytes(Label.Text);

                            Stream.Write(Bytes, 0, Bytes.Length);

                            continue;
                        }
                    case LabelType.Compressed:
                        {
                            var Flag = (ushort) Label.Type << 6;

                            var FlagOffset = (byte) (Flag + Label.Offset);

                            Stream.WriteByte(FlagOffset);

                            return;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LabelType), Label.Type, null);
                }
            }

            Stream.WriteByte(0);
        }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext Context)
        {
            var MemoryStream = GetRootStream(Stream);

            var Bytes = MemoryStream.ToArray();

            var DnStream = new DnStream(in Bytes);

            DnStream.Seek((ushort)MemoryStream.Position);

            Labels = GetLabels(DnStream);

            var BoundStream = Stream as BoundedStream;

            BoundStream.Seek(DnStream.BytePosition + 1, SeekOrigin.Begin);
        }

        private static List<ILabel> GetLabels(DnStream Stream)
        {
            var Labels = new List<ILabel>();

            while (true)
            {
                var LabelType = Stream.ReadBits(2).AsEnum<LabelType>();

                switch (LabelType)
                {
                    case LabelType.Normal:
                        {
                            var Length = Stream.ReadBits(6);

                            if (Length == 0)
                                return Labels;

                            var Label = new Label()
                            {
                                Type = LabelType,
                                Count = Length,
                                Offset = Stream.BytePosition,
                                Text = Stream.ReadString(Length)
                            };

                            Labels.Add(Label);

                            break;
                        }
                    case LabelType.Compressed:
                        {
                            var Pointer = (ushort)(Stream.ReadBits(6) + Stream.ReadByte());

                            if (Pointer >= Stream.BytePosition - 2)
                                throw new ArgumentOutOfRangeException(nameof(Pointer), Pointer, "Compressed Label Infinite Loop Detected.");

                            var Position = Stream.BytePosition;

                            Stream.Seek(Pointer);

                            var CompressedLabels = GetLabels(Stream).ToList();

                            CompressedLabels.ForEach(Label => Label.Type = LabelType);

                            Labels.AddRange(CompressedLabels);

                            Stream.Seek(Position);

                            return Labels;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LabelType), LabelType, null);
                }
            }
        }

        private static MemoryStream GetRootStream(Stream Stream)
        {
            while (Stream is BoundedStream BoundedStream)
            {
                Stream = BoundedStream.Source;
            }

            return Stream as MemoryStream;
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

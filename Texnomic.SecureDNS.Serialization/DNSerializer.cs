using System;
using System.Buffers.Binary;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using System.Collections.Generic;
using System.Linq;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core.Records;
using Texnomic.SecureDNS.Serialization.Extensions;

namespace Texnomic.SecureDNS.Serialization
{
    public static class DnSerializer
    {
        public static IMessage Deserialize(in byte[] Raw)
        {
            var Stream = new DnStream(in Raw);

            var Message = new Message
            {
                ID = Stream.ReadUShort(),

                MessageType = Stream.GetBit().AsEnum<MessageType>(),
                OperationCode = Stream.GetBits(4).AsEnum<OperationCode>(),
                AuthoritativeAnswer = Stream.GetBit().AsEnum<AuthoritativeAnswer>(),
                Truncated = Stream.GetBit().AsBool(),
                RecursionDesired = Stream.GetBit().AsBool(),

                RecursionAvailable = Stream.GetBit().AsBool(),
                Zero = Stream.GetBit(),
                AuthenticatedData = Stream.GetBit().AsBool(),
                CheckingDisabled = Stream.GetBit().AsBool(),
                ResponseCode = Stream.GetBits(4).AsEnum<ResponseCode>(),

                QuestionsCount = Stream.ReadUShort(),
                AnswersCount = Stream.ReadUShort(),
                AuthorityCount = Stream.ReadUShort(),
                AdditionalCount = Stream.ReadUShort()
            };

            Message.Questions = GetQuestions(in Stream, Message.QuestionsCount);
            Message.Answers = GetAnswers(in Stream, Message.AnswersCount);

            return Message;
        }

        public static byte[] Serialize(in IMessage Message)
        {
            var Size = SizeOf(in Message);

            var Stream = new DnStream(Size);

            var Pointers = new Dictionary<string, ushort>();

            Stream.WriteUShort(Message.ID);

            Stream.SetBit((byte)Message.MessageType);
            Stream.SetBits(4, (byte)Message.OperationCode);
            Stream.SetBit((byte)Message.AuthoritativeAnswer);
            Stream.SetBit(Convert.ToByte(Message.Truncated));
            Stream.SetBit(Convert.ToByte(Message.RecursionDesired));

            Stream.SetBit(Convert.ToByte(Message.RecursionAvailable));
            Stream.SetBit(Message.Zero);
            Stream.SetBit(Convert.ToByte(Message.AuthenticatedData));
            Stream.SetBit(Convert.ToByte(Message.CheckingDisabled));
            Stream.SetBits(4, (byte)Message.ResponseCode);

            Stream.WriteUShort((ushort)Message.Questions.Count());
            Stream.WriteUShort((ushort)Message.Answers.Count());

            Stream.WriteUShort((ushort)Message.Authority.Count());
            Stream.WriteUShort((ushort)Message.Additional.Count());

            Set(in Stream, in Pointers, Message.Questions);

            Set(in Stream, in Pointers, Message.Answers);

            Set(in Stream, in Pointers, Message.Authority);

            Set(in Stream, in Pointers, Message.Additional);

            return Stream.ToArray();
        }

        public static ushort SizeOf(in IMessage Message)
        {
            var Pointers = new SortedSet<string>();

            ushort Size = 12;

            Size += SizeOf(in Pointers, Message.Questions);
            Size += SizeOf(in Pointers, Message.Answers);
            Size += SizeOf(in Pointers, Message.Authority);
            Size += SizeOf(in Pointers, Message.Additional);

            return Size;
        }

        private static IEnumerable<IQuestion> GetQuestions(in DnStream Stream, ushort Count)
        {
            var Questions = new List<IQuestion>();

            do
            {
                var Question = GetQuestion(in Stream);

                Questions.Add(Question);
            }
            while (Questions.Count < Count);

            return Questions;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, IEnumerable<IQuestion> Questions)
        {
            foreach (var Question in Questions)
            {
                Set(in Stream, in Pointers, Question);
            }
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IEnumerable<IQuestion> Questions)
        {
            ushort Size = 0;

            foreach (var Question in Questions)
            {
                Size += SizeOf(in Pointers, Question);
            }

            return Size;
        }

        private static IQuestion GetQuestion(in DnStream Stream)
        {
            var Question = new Question()
            {
                Domain = GetDomain(in Stream),
                Type = Stream.ReadUShort().AsEnum<RecordType>(),
                Class = Stream.ReadUShort().AsEnum<RecordClass>()
            };

            return Question;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, IQuestion Question)
        {
            Set(in Stream, in Pointers, Question.Domain);

            Stream.WriteUShort((byte)Question.Type);

            Stream.WriteUShort((byte)Question.Class);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IQuestion Question)
        {
            var Size = SizeOf(in Pointers, Question.Domain);

            return (ushort)(Size + 4);
        }

        private static IDomain GetDomain(in DnStream Stream)
        {
            var Domain = new Domain()
            {
                Labels = GetLabels(in Stream),
            };

            return Domain;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, IDomain Domain)
        {
            Set(in Stream, in Pointers, Domain.Labels.ToArray());
        }

        private static ReadOnlySpan<byte> Serialize(in IDomain Domain)
        {
            var Size = Domain.Labels.Sum(Label => Label.Length) + Domain.Labels.Count();

            var Stream = new DnStream((ushort)Size);

            foreach (var Label in Domain.Labels)
            {
                Stream.SetBits(2, (byte) LabelType.Normal);

                Stream.SetBits(6, (byte) Label.Length);

                Stream.WriteString(Label);
            }

            Stream.SetByte(0);

            return Stream.ToSpan();
        }

        private static void Set(in DnStream Stream, in IDomain Domain)
        {
            var Span = Serialize(in Domain);

            byte Index = 0;

            while (Span[Index] != 0)
            {
                var Label = Span.Slice(Index);


            }
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IDomain Domain)
        {
            return SizeOf(in Pointers, Domain.Labels.ToArray());
        }

        private static IEnumerable<string> GetLabels(in DnStream Stream)
        {
            var Labels = new List<string>();

            while (true)
            {
                var LabelType = Stream.GetBits(2).AsEnum<LabelType>();

                switch (LabelType)
                {
                    case LabelType.Normal:
                        {
                            var Length = Stream.GetBits(6);

                            if (Length == 0)
                                return Labels;


                            var Label = Stream.ReadString(Length);

                            Labels.Add(Label);

                            break;
                        }
                    case LabelType.Compressed:
                        {
                            var Pointer = (ushort)(Stream.GetBits(6) + Stream.GetByte());

                            if (Pointer >= Stream.BytePosition - 2) throw new ArgumentOutOfRangeException(nameof(Pointer), Pointer, "Compressed Label Infinite Loop Detected.");

                            var Position = Stream.BytePosition;

                            Stream.Seek(Pointer);

                            Labels.AddRange(GetLabels(in Stream));

                            Stream.Seek(Position);

                            return Labels;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LabelType), LabelType, null);
                }
            }
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, ReadOnlySpan<string> Labels)
        {
            foreach (var Label in Labels)
            {
                var SubDomain = string.Join('.', Labels.Slice(Labels.IndexOf(Label)).ToArray());

                if (Pointers.ContainsKey(SubDomain))
                {
                    Stream.SetBits(2, (byte)LabelType.Compressed);

                    var Bytes = new byte[2];

                    BinaryPrimitives.WriteUInt16BigEndian(Bytes, Pointers[SubDomain]);

                    Stream.SetBits(6, Bytes[0]);

                    Stream.SetByte(Bytes[1]);

                    return;
                }
                else
                {
                    Pointers.Add(SubDomain, Stream.BytePosition);

                    Stream.SetBits(2, (byte)LabelType.Normal);

                    Stream.SetBits(6, (byte)Label.Length);

                    Stream.WriteString(Label);
                }
            }

            Stream.SetByte(0);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, ReadOnlySpan<string> Labels)
        {
            ushort Size = 0;

            foreach (var Label in Labels)
            {
                var SubDomain = string.Join('.', Labels.Slice(Labels.IndexOf(Label)).ToArray());

                if (Pointers.Contains(SubDomain))
                {
                    Size += 2;

                    return Size;
                }
                else
                {
                    Pointers.Add(SubDomain);

                    Size += (ushort)(1 + Label.Length);
                }
            }

            Size += 1;

            return Size;
        }

        private static IEnumerable<IAnswer> GetAnswers(in DnStream Stream, ushort Count)
        {
            var Answers = new List<IAnswer>();

            do
            {
                var Answer = GetAnswer(in Stream);

                Answers.Add(Answer);
            }
            while (Answers.Count < Count);

            return Answers;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, IEnumerable<IAnswer> Answers)
        {
            foreach (var Answer in Answers)
            {
                Set(in Stream, in Pointers, Answer);
            }
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IEnumerable<IAnswer> Answers)
        {
            ushort Size = 0;

            foreach (var Answer in Answers)
            {
                Size += SizeOf(in Pointers, Answer);
            }

            return Size;
        }

        private static IAnswer GetAnswer(in DnStream Stream)
        {
            var Answer = new Answer()
            {
                Domain = GetDomain(in Stream),
                Type = Stream.ReadUShort().AsEnum<RecordType>(),
                Class = Stream.ReadUShort().AsEnum<RecordClass>(),
                TimeToLive = Stream.ReadTimeSpan(),
                Length = Stream.ReadUShort(),
            };

            Answer.Record = GetRecord(in Stream, Answer.Type);

            return Answer;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, IAnswer Answer)
        {
            Set(in Stream, in Pointers, Answer.Domain);

            Stream.WriteUShort((byte)Answer.Type);

            Stream.WriteUShort((byte)Answer.Class);

            Stream.WriteTimeSpan(Answer.TimeToLive);

            Stream.WriteUShort(Answer.Length);

            Set(in Stream, in Pointers, Answer.Record);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IAnswer Answer)
        {
            ushort Size = 0;

            Size += SizeOf(in Pointers, Answer.Domain);

            Size += 10;

            Size += SizeOf(in Pointers, Answer.Record);

            return Size;
        }

        private static IRecord GetRecord(in DnStream Stream, RecordType RecordType)
        {
            return RecordType switch
            {
                RecordType.A => GetA(in Stream),
                RecordType.CNAME => GetCNAME(in Stream),
                RecordType.AAAA => GetAAAA(in Stream),
                _ => throw new ArgumentOutOfRangeException(nameof(RecordType), RecordType, null)
            };
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, IRecord Record)
        {
            switch (Record)
            {
                case A A:
                    {
                        Set(in Stream, in A);
                        break;
                    }
                case CNAME CNAME:
                    {
                        Set(in Stream, in Pointers, in CNAME);
                        break;
                    }
                case AAAA AAAA:
                    {
                        Set(in Stream, in AAAA);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(Record), Record, null);
            }
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IRecord Record)
        {
            switch (Record)
            {
                case A A:
                    {
                        return SizeOf(in A);
                    }
                case CNAME CNAME:
                    {
                        return SizeOf(in Pointers, in CNAME);
                    }
                case AAAA AAAA:
                    {
                        return SizeOf(in AAAA);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(Record), Record, null);
            }
        }

        private static CNAME GetCNAME(in DnStream Stream)
        {
            return new CNAME()
            {
                Domain = GetDomain(in Stream)
            };
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, in CNAME CNAME)
        {
            Set(in Stream, in Pointers, CNAME.Domain);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, in CNAME CNAME)
        {
            return SizeOf(in Pointers, CNAME.Domain);
        }

        private static A GetA(in DnStream Stream)
        {
            return new A()
            {
                Address = Stream.ReadIPv4Address()
            };
        }

        private static void Set(in DnStream Stream, in A A)
        {
            Stream.WriteIPv4Address(A.Address);
        }

        private static ushort SizeOf(in A A)
        {
            return 4;
        }

        private static AAAA GetAAAA(in DnStream Stream)
        {
            return new AAAA()
            {
                Address = Stream.ReadIPv6Address()
            };
        }

        private static void Set(in DnStream Stream, in AAAA AAAA)
        {
            Stream.WriteIPv6Address(AAAA.Address);
        }

        private static ushort SizeOf(in AAAA AAAA)
        {
            return 8;
        }

        private static ICharacterString GetCharacterString(in DnStream Stream)
        {
            var Length = Stream.GetByte();

            return new CharacterString()
            {
                Length = Length,
                Value = Stream.ReadString(Length)
            };
        }

        private static void Set(in DnStream Stream, in ICharacterString CharacterString)
        {
            Stream.SetByte(CharacterString.Length);
            Stream.WriteString(CharacterString.Value);
        }

        private static ushort SizeOf(in ICharacterString CharacterString)
        {
            return (ushort)(CharacterString.Length + 1);
        }

        private static Certificate GetCertificate(in DnStream Stream)
        {
            return new Certificate()
            {
                Magic = Stream.ReadString(4),
                Version = Stream.ReadUShort().AsEnum<ESVersion>(),
                MinorVersion = Stream.ReadUShort(),
                Signature = Stream.ReadBytes(64).ToArray(),
                PublicKey = Stream.ReadBytes(32).ToArray(),
                ClientMagic = Stream.ReadBytes(8).ToArray(),
                Serial = Stream.ReadInt32(),
                StartTimeStamp = Stream.ReadEpoch(),
                EndTimeStamp = Stream.ReadEpoch(),
                //Extensions = Stream.GetBytes()
            };
        }

        private static void Set(in DnStream Stream, in ICertificate Certificate)
        {
            Stream.WriteString(Certificate.Magic);
            Stream.WriteUShort((ushort)Certificate.Version);
            Stream.WriteUShort(Certificate.MinorVersion);
            Stream.WriteBytes(Certificate.Signature);
            Stream.WriteBytes(Certificate.PublicKey);
            Stream.WriteBytes(Certificate.ClientMagic);
            Stream.WriteInt32(Certificate.Serial);
            Stream.WriteEpoch(Certificate.StartTimeStamp);
            Stream.WriteEpoch(Certificate.EndTimeStamp);
            //Stream.SetBytes(Certificate.Extensions);
        }

        private static ushort SizeOf(in ICertificate Certificate)
        {
            return (ushort)(124 + Certificate.Extensions.Length);
        }
    }
}

using System;
using System.Buffers.Binary;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
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

        #region Questions

        private static IEnumerable<IQuestion> GetQuestions(in DnStream Stream, ushort Count)
        {
            var Questions = new List<IQuestion>();

            do
            {
                var Question = GetQuestion(in Stream);

                Questions.Add(Question);
            } while (Questions.Count < Count);

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

        #endregion

        #region Question

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

        #endregion

        #region Domain

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
                Stream.SetBits(2, (byte)LabelType.Normal);

                Stream.SetBits(6, (byte)Label.Length);

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

        #endregion

        #region Answers

        private static IEnumerable<IAnswer> GetAnswers(in DnStream Stream, ushort Count)
        {
            var Answers = new List<IAnswer>();

            do
            {
                var Answer = GetAnswer(in Stream);

                Answers.Add(Answer);
            } while (Answers.Count < Count);

            return Answers;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers,
            IEnumerable<IAnswer> Answers)
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

        #endregion

        #region Answer

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

        #endregion

        #region Record

        private static IRecord GetRecord(in DnStream Stream, RecordType RecordType)
        {
            return RecordType switch
            {
                RecordType.A => GetA(in Stream),
                RecordType.CNAME => GetCNAME(in Stream),
                RecordType.AAAA => GetAAAA(in Stream),
                RecordType.TXT => GetTXT(in Stream),
                RecordType.MX => GetMX(in Stream),
                RecordType.NS => GetNS(in Stream),
                RecordType.PTR => GetPTR(in Stream),
                RecordType.SOA => GetSOA(in Stream),
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
                case TXT TXT:
                    {
                        Set(in Stream, in TXT);
                        break;
                    }
                case MX MX:
                    {
                        Set(in Stream, in MX);
                        break;
                    }
                case NS NS:
                    {
                        Set(in Stream, in NS);
                        break;
                    }
                case PTR PTR:
                    {
                        Set(in Stream, in PTR);
                        break;
                    }
                case SOA SOA:
                    {
                        Set(in Stream, in SOA);
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

        #endregion

        #region A

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

        #endregion

        #region CNAME

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

        #endregion

        #region AAAA

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

        #endregion

        #region TXT

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
                Length = Stream.GetByte(),
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
            Stream.SetByte(Certificate.Length);
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



        private static TXT GetTXT(in DnStream Stream)
        {
            var Length = Stream.GetByte();

            var Magic = Stream.ReadString(4);

            Stream.Seek((ushort)(Stream.BytePosition - 5));

            if (Magic == "DNSC")
            {
                return new TXT()
                {
                    Certificate = GetCertificate(in Stream)
                };
            }

            return new TXT()
            {
                Text = GetCharacterString(in Stream)
            };
        }

        private static void Set(in DnStream Stream, in TXT TXT)
        {
            if (TXT.Certificate is null)
            {
                Set(in Stream, TXT.Text);
            }
            else
            {
                Set(in Stream, TXT.Certificate);
            }
        }

        private static ushort SizeOf(in DnStream Stream, in TXT TXT)
        {
            return TXT.Certificate is null ? SizeOf(TXT.Text) : SizeOf(TXT.Certificate);
        }

        #endregion

        #region MX

        private static MX GetMX(in DnStream Stream)
        {
            return new MX()
            {
                Preference = Stream.ReadShort(),
                Exchange = GetDomain(in Stream)
            };
        }

        private static void Set(in DnStream Stream, in MX MX)
        {
            Stream.WriteShort(MX.Preference);
            Set(in Stream, MX.Exchange);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, in MX MX)
        {
            return (ushort)(2 + SizeOf(Pointers, MX.Exchange));
        }

        #endregion

        #region NS

        private static NS GetNS(in DnStream Stream)
        {
            return new NS()
            {
                Domain = GetDomain(in Stream)
            };
        }

        private static void Set(in DnStream Stream, in NS NS)
        {
            Set(in Stream, NS.Domain);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, in NS NS)
        {
            return SizeOf(in Pointers, NS.Domain);
        }

        #endregion

        #region PTR

        private static PTR GetPTR(in DnStream Stream)
        {
            return new PTR()
            {
                Domain = GetDomain(in Stream)
            };
        }

        private static void Set(in DnStream Stream, in PTR PTR)
        {
            Set(in Stream, PTR.Domain);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, in PTR PTR)
        {
            return SizeOf(in Pointers, PTR.Domain);
        }

        #endregion

        #region SOA

        private static SOA GetSOA(in DnStream Stream)
        {
            return new SOA()
            {
                PrimaryNameServer = GetDomain(in Stream),
                ResponsibleAuthorityMailbox = GetDomain(in Stream),
                SerialNumber = Stream.ReadUInt32(),
                RefreshInterval = Stream.ReadTimeSpan(),
                RetryInterval = Stream.ReadTimeSpan(),
                ExpiryLimit = Stream.ReadTimeSpan(),
                TimeToLive = Stream.ReadTimeSpan(),
            };
        }

        private static void Set(in DnStream Stream, in SOA SOA)
        {
            Set(in Stream, SOA.PrimaryNameServer);
            Set(in Stream, SOA.ResponsibleAuthorityMailbox);
            Stream.WriteUInt32(SOA.SerialNumber);
            Stream.WriteTimeSpan(SOA.RefreshInterval);
            Stream.WriteTimeSpan(SOA.RetryInterval);
            Stream.WriteTimeSpan(SOA.ExpiryLimit);
            Stream.WriteTimeSpan(SOA.TimeToLive);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, in SOA SOA)
        {
            return (ushort)(SizeOf(in Pointers, SOA.PrimaryNameServer) + SizeOf(in Pointers, SOA.ResponsibleAuthorityMailbox) + 4 * 5);
        }

        #endregion
    }
}

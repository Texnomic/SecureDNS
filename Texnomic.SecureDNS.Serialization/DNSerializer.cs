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
        public static IMessage Deserialize(ref byte[] Raw)
        {
            var Stream = new DnStream(ref Raw);

            var Message = new Message
            {
                ID = Stream.GetUShort(),

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

                QuestionsCount = Stream.GetUShort(),
                AnswersCount = Stream.GetUShort(),
                AuthorityCount = Stream.GetUShort(),
                AdditionalCount = Stream.GetUShort()
            };

            Message.Questions = GetQuestions(ref Stream, Message.QuestionsCount);
            Message.Answers = GetAnswers(ref Stream, Message.AnswersCount);

            return Message;
        }

        public static byte[] Serialize(ref IMessage Message)
        {
            var Size = SizeOf(ref Message);

            var Stream = new DnStream(Size);

            var Pointers = new Dictionary<string, ushort>();

            Stream.SetUShort(Message.ID);

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

            Stream.SetUShort((ushort)Message.Questions.Count());
            Stream.SetUShort((ushort)Message.Answers.Count());

            Stream.SetUShort((ushort)Message.Authority.Count());
            Stream.SetUShort((ushort)Message.Additional.Count());

            Set(ref Stream, ref Pointers, Message.Questions);

            Set(ref Stream, ref Pointers, Message.Answers);

            Set(ref Stream, ref Pointers, Message.Authority);

            Set(ref Stream, ref Pointers, Message.Additional);

            return Stream.ToArray();
        }

        public static ushort SizeOf(ref IMessage Message)
        {
            var Pointers = new List<string>();

            ushort Size = 12;

            Size += SizeOf(ref Pointers, Message.Questions);
            Size += SizeOf(ref Pointers, Message.Answers);
            Size += SizeOf(ref Pointers, Message.Authority);
            Size += SizeOf(ref Pointers, Message.Additional);

            return Size;
        }

        private static IEnumerable<IQuestion> GetQuestions(ref DnStream Stream, ushort Count)
        {
            var Questions = new List<IQuestion>();

            do
            {
                var Question = GetQuestion(ref Stream);

                Questions.Add(Question);
            }
            while (Questions.Count < Count);

            return Questions;
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, IEnumerable<IQuestion> Questions)
        {
            foreach (var Question in Questions)
            {
                Set(ref Stream, ref Pointers, Question);
            }
        }

        private static ushort SizeOf(ref List<string> Pointers, IEnumerable<IQuestion> Questions)
        {
            ushort Size = 0;

            foreach (var Question in Questions)
            {
                Size += SizeOf(ref Pointers, Question);
            }

            return Size;
        }

        private static IQuestion GetQuestion(ref DnStream Stream)
        {
            var Question = new Question()
            {
                Domain = GetDomain(ref Stream),
                Type = Stream.GetUShort().AsEnum<RecordType>(),
                Class = Stream.GetUShort().AsEnum<RecordClass>()
            };

            return Question;
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, IQuestion Question)
        {
            Set(ref Stream, ref Pointers, Question.Domain);

            Stream.SetUShort((byte)Question.Type);

            Stream.SetUShort((byte)Question.Class);
        }

        private static ushort SizeOf(ref List<string> Pointers, IQuestion Question)
        {
            var Size = SizeOf(ref Pointers, Question.Domain);

            return (ushort)(Size + 4);
        }

        private static IDomain GetDomain(ref DnStream Stream)
        {
            var Domain = new Domain()
            {
                Labels = GetLabels(ref Stream),
            };

            return Domain;
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, IDomain Domain)
        {
            Set(ref Stream, ref Pointers, Domain.Labels);
        }

        private static ushort SizeOf(ref List<string> Pointers, IDomain Domain)
        {
            return SizeOf(ref Pointers, Domain.Labels);
        }

        private static IEnumerable<string> GetLabels(ref DnStream Stream)
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


                            var Label = Stream.GetString(Length);

                            Labels.Add(Label);

                            break;
                        }
                    case LabelType.Compressed:
                        {
                            var Pointer = (ushort)(Stream.GetBits(6) + Stream.GetByte());

                            if (Pointer >= Stream.BytePosition - 2) throw new ArgumentOutOfRangeException(nameof(Pointer), Pointer, "Compressed Label Infinite Loop Detected.");

                            var Position = Stream.BytePosition;

                            Stream.Seek(Pointer);

                            Labels.AddRange(GetLabels(ref Stream));

                            Stream.Seek(Position);

                            return Labels;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LabelType), LabelType, null);
                }
            }
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, IEnumerable<string> Labels)
        {
            var Domain = string.Join('.', Labels);

            foreach (var Label in Labels)
            {
                var SubDomain = Domain.Substring(Domain.IndexOf(Label));

                if (Pointers.ContainsKey(SubDomain))
                {
                    Stream.SetBits(2, (byte) LabelType.Compressed);

                    var Bytes = new byte[2];

                    BinaryPrimitives.WriteUInt16BigEndian(Bytes, Pointers[SubDomain]);

                    Stream.SetBits(6, Bytes[0]);

                    Stream.SetByte(Bytes[1]);

                    return;
                }
                else
                {
                    Pointers.Add(SubDomain, Stream.BytePosition);

                    Stream.SetBits(2, (byte) LabelType.Normal);

                    Stream.SetBits(6, (byte) Label.Length);

                    Stream.SetString(Label);
                }
            }

            Stream.SetByte(0);
        }

        private static ushort SizeOf(ref List<string> Pointers, IEnumerable<string> Labels)
        {
            var Domain = string.Join('.', Labels);

            ushort Size = 0;

            foreach (var Label in Labels)
            {
                var SubDomain = Domain.Substring(Domain.IndexOf(Label));

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

        private static IEnumerable<IAnswer> GetAnswers(ref DnStream Stream, ushort Count)
        {
            var Answers = new List<IAnswer>();

            do
            {
                var Answer = GetAnswer(ref Stream);

                Answers.Add(Answer);
            }
            while (Answers.Count < Count);

            return Answers;
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, IEnumerable<IAnswer> Answers)
        {
            foreach (var Answer in Answers)
            {
                Set(ref Stream, ref Pointers, Answer);
            }
        }

        private static ushort SizeOf(ref List<string> Pointers, IEnumerable<IAnswer> Answers)
        {
            ushort Size = 0;

            foreach (var Answer in Answers)
            {
                Size += SizeOf(ref Pointers, Answer);
            }

            return Size;
        }

        private static IAnswer GetAnswer(ref DnStream Stream)
        {
            var Answer = new Answer()
            {
                Domain = GetDomain(ref Stream),
                Type = Stream.GetUShort().AsEnum<RecordType>(),
                Class = Stream.GetUShort().AsEnum<RecordClass>(),
                TimeToLive = Stream.GetTimeSpan(),
                Length = Stream.GetUShort(),
            };

            Answer.Record = GetRecord(ref Stream, Answer.Type);

            return Answer;
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, IAnswer Answer)
        {
            Set(ref Stream, ref Pointers, Answer.Domain);

            Stream.SetUShort((byte)Answer.Type);

            Stream.SetUShort((byte)Answer.Class);

            Stream.SetTimeSpan(Answer.TimeToLive);

            Stream.SetUShort(Answer.Length);

            Set(ref Stream, ref Pointers, Answer.Record);
        }

        private static ushort SizeOf(ref List<string> Pointers, IAnswer Answer)
        {
            ushort Size = 0;

            Size += SizeOf(ref Pointers, Answer.Domain);

            Size += 10;

            Size += SizeOf(ref Pointers, Answer.Record);

            return Size;
        }

        private static IRecord GetRecord(ref DnStream Stream, RecordType RecordType)
        {
            switch (RecordType)
            {
                case RecordType.A: return GetA(ref Stream);

                case RecordType.NS:

                case RecordType.MD:

                case RecordType.MF:

                case RecordType.CNAME: return GetCNAME(ref Stream);


                case RecordType.SOA:

                case RecordType.MB:

                case RecordType.MG:

                case RecordType.MR:

                case RecordType.NULL:

                case RecordType.WKS:

                case RecordType.PTR:

                case RecordType.HINFO:

                case RecordType.MINFO:

                case RecordType.MX:

                case RecordType.TXT:

                case RecordType.RP:

                case RecordType.AFSDB:

                case RecordType.X25:

                case RecordType.ISDN:

                case RecordType.RT:

                case RecordType.NSAP:

                case RecordType.NSAP_PTR:

                case RecordType.SIG:

                case RecordType.KEY:

                case RecordType.PX:

                case RecordType.GPOS:

                case RecordType.AAAA: return GetAAAA(ref Stream);

                case RecordType.LOC:

                case RecordType.NXT:

                case RecordType.EID:

                case RecordType.NIMLOC:

                case RecordType.SRV:

                case RecordType.ATMA:

                case RecordType.NAPTR:

                case RecordType.KX:

                case RecordType.CERT:

                case RecordType.A6:

                case RecordType.DNAME:

                case RecordType.SINK:

                case RecordType.OPT:

                case RecordType.APL:

                case RecordType.DS:

                case RecordType.SSHFP:

                case RecordType.IPSECKEY:

                case RecordType.RRSIG:

                case RecordType.NSEC:

                case RecordType.DNSKEY:

                case RecordType.DHCID:

                case RecordType.NSEC3:

                case RecordType.NSEC3PARAM:

                case RecordType.TLSA:

                case RecordType.SMIMEA:

                case RecordType.HIP:

                case RecordType.NINFO:

                case RecordType.RKEY:

                case RecordType.CDS:

                case RecordType.CDNSKEY:

                case RecordType.OPENPGPKEY:

                case RecordType.SPF:

                case RecordType.UINFO:

                case RecordType.UID:

                case RecordType.GID:

                case RecordType.UNSPEC:

                case RecordType.TKEY:

                case RecordType.TSIG:

                case RecordType.IXFR:

                case RecordType.AXFR:

                case RecordType.MAILB:

                case RecordType.MAILA:

                case RecordType.Any:

                case RecordType.URI:

                case RecordType.CAA:

                case RecordType.AVC:

                case RecordType.DOA:

                case RecordType.AMTRELAY:

                case RecordType.TA:

                case RecordType.DLV:

                default:
                    throw new ArgumentOutOfRangeException(nameof(RecordType), RecordType, null);
            }
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, IRecord Record)
        {
            switch (Record)
            {
                case A A:
                    {
                        Set(ref Stream, ref A);
                        break;
                    }
                case CNAME CNAME:
                    {
                        Set(ref Stream, ref Pointers, ref CNAME);
                        break;
                    }
                case AAAA AAAA:
                    {
                        Set(ref Stream, ref AAAA);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(Record), Record, null);
            }
        }

        private static ushort SizeOf(ref List<string> Pointers, IRecord Record)
        {
            switch (Record)
            {
                case A A:
                {
                    return SizeOf(ref A);
                }
                case CNAME CNAME:
                {
                    return SizeOf(ref Pointers, ref CNAME);
                }
                case AAAA AAAA:
                {
                    return SizeOf(ref AAAA);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(Record), Record, null);
            }
        }

        private static CNAME GetCNAME(ref DnStream Stream)
        {
            return new CNAME()
            {
                Domain = GetDomain(ref Stream)
            };
        }

        private static void Set(ref DnStream Stream, ref Dictionary<string, ushort> Pointers, ref CNAME CNAME)
        {
            Set(ref Stream, ref Pointers, CNAME.Domain);
        }

        private static ushort SizeOf(ref List<string> Pointers, ref CNAME CNAME)
        {
            return SizeOf(ref Pointers, CNAME.Domain);
        }

        private static A GetA(ref DnStream Stream)
        {
            return new A()
            {
                Address = Stream.GetIPv4Address()
            };
        }

        private static void Set(ref DnStream Stream, ref A A)
        {
            Stream.SetIPv4Address(A.Address);
        }

        private static ushort SizeOf(ref A A)
        {
            return 4;
        }

        private static AAAA GetAAAA(ref DnStream Stream)
        {
            return new AAAA()
            {
                Address = Stream.GetIPv6Address()
            };
        }

        private static void Set(ref DnStream Stream, ref AAAA AAAA)
        {
            Stream.SetIPv6Address(AAAA.Address);
        }

        private static ushort SizeOf(ref AAAA AAAA)
        {
            return 8;
        }
    }
}

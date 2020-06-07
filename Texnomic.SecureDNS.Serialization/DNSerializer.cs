using System;
using System.Linq;
using System.Buffers.Binary;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using System.Collections.Generic;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core.Records;
using Texnomic.SecureDNS.Extensions;

namespace Texnomic.SecureDNS.Serialization
{
    public static class DnSerializer
    {
        public static IMessage Deserialize(in byte[] Raw)
        {
            var Stream = new DnStream(in Raw);

            return GetMessage(in Stream);
        }

        public static IDnStamp Deserialize(in string Stamp)
        {
            if (!Stamp.StartsWith("sdns://")) throw new ArgumentException("Stamp Uri Must Start With SDNS://");

            var Raw = Decode(Stamp[7..]);

            var Stream = new DnStream(in Raw);

            return GetStamp(in Stream);
        }

        private static IMessage GetMessage(in DnStream Stream)
        {
            if (Stream.Length <= 12)
                throw new FormatException($"Incomplete DNS Message, Header Less Than 12 Bytes, Got {Stream.Length}.");

            var Message = new Message
            {
                ID = Stream.ReadUShort(),

                MessageType = Stream.ReadBit().AsEnum<MessageType>(),
                OperationCode = Stream.ReadBits(4).AsEnum<OperationCode>(),
                AuthoritativeAnswer = Stream.ReadBit().AsEnum<AuthoritativeAnswer>(),
                Truncated = Stream.ReadBit().AsBool(),
                RecursionDesired = Stream.ReadBit().AsBool(),

                RecursionAvailable = Stream.ReadBit().AsBool(),
                Zero = Stream.ReadBit(),
                AuthenticatedData = Stream.ReadBit().AsBool(),
                CheckingDisabled = Stream.ReadBit().AsBool(),
                ResponseCode = Stream.ReadBits(4).AsEnum<ResponseCode>(),

                QuestionsCount = Stream.ReadUShort(),
                AnswersCount = Stream.ReadUShort(),
                AuthorityCount = Stream.ReadUShort(),
                AdditionalCount = Stream.ReadUShort()
            };

            if (Message.QuestionsCount == 0)
                throw new FormatException("Incomplete DNS Message, Message Must Have At Least 1 Question Record.");

            Message.Questions = GetQuestions(in Stream, Message.QuestionsCount);

            if (Message.MessageType == MessageType.Query) return Message;

            if (Message.Truncated)
                throw new FormatException("Truncated DNS Message.");

            if (Message.ResponseCode == ResponseCode.NoError &&
                Message.AnswersCount == 0 &&
                Message.AuthorityCount == 0 &&
                Message.AdditionalCount == 0)
                throw new FormatException("Incomplete DNS Message, Response Must Have At Least 1 Answer Record.");

            Message.Answers = GetAnswers(in Stream, Message.AnswersCount);

            Message.Authority = GetAnswers(in Stream, Message.AuthorityCount);

            Message.Additional = GetAnswers(in Stream, Message.AdditionalCount);

            return Message;
        }

        public static byte[] Serialize(in IMessage Message)
        {
            var Size = SizeOf(in Message);

            var Stream = new DnStream(Size);

            var Pointers = new Dictionary<string, ushort>();

            Stream.WriteUShort(Message.ID);

            Stream.WriteBit((byte)Message.MessageType);
            Stream.WriteBits(4, (byte)Message.OperationCode);
            Stream.WriteBit((byte)Message.AuthoritativeAnswer);
            Stream.WriteBit(Convert.ToByte(Message.Truncated));
            Stream.WriteBit(Convert.ToByte(Message.RecursionDesired));

            Stream.WriteBit(Convert.ToByte(Message.RecursionAvailable));
            Stream.WriteBit(Message.Zero);
            Stream.WriteBit(Convert.ToByte(Message.AuthenticatedData));
            Stream.WriteBit(Convert.ToByte(Message.CheckingDisabled));
            Stream.WriteBits(4, (byte)Message.ResponseCode);

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

        public static byte[] Serialize(ICertificate Certificate)
        {
            var Size = SizeOf(in Certificate);

            var Stream = new DnStream(Size);

            Set(in Stream, Certificate);

            return Stream.ToArray();
        }

        public static byte[] Serialize(IDnStamp Stamp)
        {
            var Size = SizeOf(Stamp);

            var Stream = new DnStream(Size);

            Set(in Stream, Stamp);

            return Stream.ToArray();
        }

        #region DnStamp

        private static string Encode(byte[] Stamp)
        {
            return Convert.ToBase64String(Stamp)
                .Replace("=", "")
                .Replace("/", "_")
                .Replace("+", "-");
        }

        private static string ToBase64(string Stamp)
        {
            return Stamp
                .PadRight(Stamp.Length + (4 - Stamp.Length % 4) % 4, '=')
                .Replace("_", "/")
                .Replace("-", "+");
        }

        private static byte[] Decode(string Stamp)
        {
            return Convert.FromBase64String(ToBase64(Stamp));
        }

        private static IDnStamp GetStamp(in DnStream Stream)
        {
            var DnStamp = new DnStamp()
            {
                Protocol = Stream.ReadByte().AsEnum<StampProtocol>(),
            };

            DnStamp.Value = GetStamp(in Stream, DnStamp.Protocol);

            return DnStamp;
        }

        private static void Set(in DnStream Stream, IDnStamp Stamp)
        {
            Stream.WriteByte((byte)Stamp.Protocol);

            switch (Stamp)
            {
                case DNSCryptRelayStamp DNSCryptRelayStamp:
                    {
                        Set(in Stream, in DNSCryptRelayStamp);
                        break;
                    }
                case DNSCryptStamp DNSCryptStamp:
                    {
                        Set(in Stream, in DNSCryptStamp);
                        break;
                    }
                case DoHStamp DoHStamp:
                    {
                        Set(in Stream, in DoHStamp);
                        break;
                    }
                case DoTStamp DoTStamp:
                    {
                        Set(in Stream, in DoTStamp);
                        break;
                    }
                case DoUStamp DoUStamp:
                    {
                        Set(in Stream, in DoUStamp);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(Stamp), Stamp, null);
            }
        }

        private static ushort SizeOf(IDnStamp Stamp)
        {
            var Size = Stamp switch
            {
                DNSCryptRelayStamp DNSCryptRelayStamp => SizeOf(in DNSCryptRelayStamp),
                DNSCryptStamp DNSCryptStamp => SizeOf(in DNSCryptStamp),
                DoHStamp DoHStamp => SizeOf(in DoHStamp),
                DoTStamp DoTStamp => SizeOf(in DoTStamp),
                DoUStamp DoUStamp => SizeOf(in DoUStamp),
                _ => throw new ArgumentOutOfRangeException(nameof(Stamp), Stamp, null)
            };

            return (ushort)(Size + 1);
        }

        private static IStamp GetStamp(in DnStream Stream, StampProtocol Protocol)
        {
            return Protocol switch
            {
                StampProtocol.DNSCryptRelay => GetDNSCryptRelayStamp(in Stream),
                StampProtocol.DNSCrypt => GetDNSCryptStamp(in Stream),
                StampProtocol.DoH => GetDoTStamp(in Stream),
                StampProtocol.DoT => GetDoTStamp(in Stream),
                StampProtocol.DoU => GetDoUStamp(in Stream),
                _ => throw new ArgumentOutOfRangeException(nameof(StampProtocol), Protocol, null)
            };
        }

        #endregion

        #region DNSCryptStamp

        private static IStamp GetDNSCryptStamp(in DnStream Stream)
        {
            return new DNSCryptStamp()
            {
                DnsSec = Stream.ReadByte().AsBool(),
                NoLog = Stream.ReadByte().AsBool(),
                NoFilter = Stream.ReadByte().AsBool(),
                Flags = Stream.ReadBytes(5).ToArray(),
                Address = Stream.ReadPrefixedString(),
                PublicKey = Stream.ReadPrefixedBytes().ToArray(),
                ProviderName = Stream.ReadPrefixedString()
            };
        }

        private static void Set(in DnStream Stream, in DNSCryptStamp Stamp)
        {
            Stream.WriteByte(Stamp.DnsSec.AsByte());
            Stream.WriteByte(Stamp.NoLog.AsByte());
            Stream.WriteByte(Stamp.NoFilter.AsByte());
            Stream.WriteBytes(Stamp.Flags);
            Stream.WritePrefixedString(Stamp.Address);
            Stream.WritePrefixedBytes(Stamp.PublicKey);
            Stream.WritePrefixedString(Stamp.ProviderName);
        }

        private static ushort SizeOf(in DNSCryptStamp Stamp)
        {
            return (ushort)(8 + Stamp.Address.Length + Stamp.PublicKey.Length + Stamp.ProviderName.Length);
        }


        #endregion

        #region DNSCryptRelayStamp
        private static IStamp GetDNSCryptRelayStamp(in DnStream Stream)
        {
            return new DNSCryptRelayStamp()
            {
                Address = Stream.ReadPrefixedString()
            };
        }

        private static void Set(in DnStream Stream, in DNSCryptRelayStamp Stamp)
        {
            Stream.WritePrefixedString(Stamp.Address);
        }

        private static ushort SizeOf(in DNSCryptRelayStamp Stamp)
        {
            return (ushort)Stamp.Address.Length;
        }

        #endregion

        #region DoHStamp

        private static IStamp GetDoHStamp(in DnStream Stream)
        {
            return new DoHStamp()
            {
                DnsSec = Stream.ReadByte().AsBool(),
                NoLog = Stream.ReadByte().AsBool(),
                NoFilter = Stream.ReadByte().AsBool(),
                Flags = Stream.ReadBytes(5).ToArray(),
                Address = Stream.ReadPrefixedString(),
                Hash = Stream.ReadPrefixedBytes().ToArray(),
                Hostname = Stream.ReadPrefixedString(),
                Path = Stream.ReadPrefixedString()
            };
        }

        private static void Set(in DnStream Stream, in DoHStamp Stamp)
        {
            Stream.WriteByte(Stamp.DnsSec.AsByte());
            Stream.WriteByte(Stamp.NoLog.AsByte());
            Stream.WriteByte(Stamp.NoFilter.AsByte());
            Stream.WriteBytes(Stamp.Flags);
            Stream.WritePrefixedString(Stamp.Address);
            Stream.WritePrefixedBytes(Stamp.Hash);
            Stream.WritePrefixedString(Stamp.Hostname);
            Stream.WritePrefixedString(Stamp.Path);
        }

        private static ushort SizeOf(in DoHStamp Stamp)
        {
            return (ushort)(9 + Stamp.Address.Length + Stamp.Hostname.Length + Stamp.Path.Length + Stamp.Hash.Length);
        }

        #endregion

        #region DoTStamp

        private static IStamp GetDoTStamp(in DnStream Stream)
        {
            return new DoTStamp()
            {
                DnsSec = Stream.ReadByte().AsBool(),
                NoLog = Stream.ReadByte().AsBool(),
                NoFilter = Stream.ReadByte().AsBool(),
                Flags = Stream.ReadBytes(5).ToArray(),
                Address = Stream.ReadPrefixedString(),
                Hash = Stream.ReadPrefixedBytes().ToArray(),
                Hostname = Stream.ReadPrefixedString()
            };
        }
        private static void Set(in DnStream Stream, in DoTStamp Stamp)
        {
            Stream.WriteByte(Stamp.DnsSec.AsByte());
            Stream.WriteByte(Stamp.NoLog.AsByte());
            Stream.WriteByte(Stamp.NoFilter.AsByte());
            Stream.WriteBytes(Stamp.Flags);
            Stream.WritePrefixedString(Stamp.Address);
            Stream.WritePrefixedBytes(Stamp.Hash);
            Stream.WritePrefixedString(Stamp.Hostname);
        }

        private static ushort SizeOf(in DoTStamp Stamp)
        {
            return (ushort)(9 + Stamp.Address.Length + Stamp.Hostname.Length + Stamp.Hash.Length);
        }

        #endregion

        #region DoUStamp

        private static IStamp GetDoUStamp(in DnStream Stream)
        {
            return new DoUStamp()
            {
                DnsSec = Stream.ReadByte().AsBool(),
                NoLog = Stream.ReadByte().AsBool(),
                NoFilter = Stream.ReadByte().AsBool(),
                Flags = Stream.ReadBytes(5).ToArray(),
                Address = Stream.ReadPrefixedString()
            };
        }

        private static void Set(in DnStream Stream, in DoUStamp Stamp)
        {
            Stream.WriteByte(Stamp.DnsSec.AsByte());
            Stream.WriteByte(Stamp.NoLog.AsByte());
            Stream.WriteByte(Stamp.NoFilter.AsByte());
            Stream.WriteBytes(Stamp.Flags);
            Stream.WritePrefixedString(Stamp.Address);
        }

        private static ushort SizeOf(in DoUStamp Stamp)
        {
            return (ushort)(8 + Stamp.Address.Length);
        }

        #endregion

        #region Questions

        private static IEnumerable<IQuestion> GetQuestions(in DnStream Stream, ushort Count)
        {
            if (Count < 1)
                throw new FormatException($"Incomplete DNS Message, DNS Message Must Have At Least 1 Question, Got {Count}.");

            var Questions = new List<IQuestion>();

            for (var i = 0; i < Count; i++)
            {
                var Question = GetQuestion(in Stream);

                Questions.Add(Question);
            }

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
                Domain = GetDomain(in Stream)
            };

            if (Stream.Length < Stream.BytePosition + 2)
                throw new FormatException("Incomplete DNS Message, Missing Question Record Type.");

            Question.Type = Stream.ReadUShort().AsEnum<RecordType>();

            //Note: Ignores Missing Bytes Error As We Can Recover Message in Single Question Cases.
            Question.Class = Stream.Length >= Stream.BytePosition + 2
                ? Stream.ReadUShort().AsEnum<RecordClass>()
                : RecordClass.Internet;

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
        private static ushort SizeOf(in SortedSet<string> Pointers, IDomain Domain)
        {
            return SizeOf(in Pointers, Domain.Labels.ToArray());
        }

        private static IEnumerable<string> GetLabels(in DnStream Stream)
        {
            var Labels = new List<string>();

            while (true)
            {
                if (Stream.Length < Stream.BytePosition + 1)
                    throw new FormatException("Incomplete DNS Message, Missing Entire Domain.");

                var LabelType = Stream.ReadBits(2).AsEnum<LabelType>();

                switch (LabelType)
                {
                    case LabelType.Normal:
                        {
                            var Length = Stream.ReadBits(6);

                            if (Length == 0)
                                return Labels;

                            if (Stream.Length < Stream.BytePosition + Length)
                                throw new FormatException("Incomplete DNS Message, Missing Normal Label.");

                            var Label = Stream.ReadString(Length);

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

                            Labels.AddRange(GetLabels(in Stream));

                            Stream.Seek(Position);

                            return Labels;
                        }
                    case LabelType.Extended:
                    case LabelType.Unallocated:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LabelType), LabelType, null);
                }
            }
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, ReadOnlySpan<string> Labels)
        {
            for (var Index = 0; Index < Labels.Length; Index++)
            {
                var SubDomain = string.Join('.', Labels[Index..].ToArray());

                if (Pointers.ContainsKey(SubDomain))
                {
                    Stream.WriteBits(2, (byte)LabelType.Compressed);

                    var Bytes = new byte[2];

                    BinaryPrimitives.WriteUInt16BigEndian(Bytes, Pointers[SubDomain]);

                    Stream.WriteBits(6, Bytes[0]);

                    Stream.WriteByte(Bytes[1]);

                    return;
                }
                else
                {
                    Pointers.Add(SubDomain, Stream.BytePosition);

                    Stream.WriteBits(2, (byte)LabelType.Normal);

                    Stream.WriteBits(6, (byte)Labels[Index].Length);

                    Stream.WriteString(Labels[Index]);
                }
            }

            Stream.WriteByte(0);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, ReadOnlySpan<string> Labels)
        {
            ushort Size = 0;

            for (var Index = 0; Index < Labels.Length; Index++)
            {
                var SubDomain = string.Join('.', Labels[Index..].ToArray());

                if (Pointers.Contains(SubDomain))
                {
                    Size += 2;

                    return Size;
                }

                Pointers.Add(SubDomain);

                Size += (ushort)(1 + Labels[Index].Length);
            }

            Size += 1;

            return Size;
        }

        #endregion

        #region Answers

        private static IEnumerable<IAnswer> GetAnswers(in DnStream Stream, ushort Count)
        {
            var Answers = new List<IAnswer>();

            for (var i = 0; i < Count; i++)
            {
                var Answer = GetAnswer(in Stream);

                Answers.Add(Answer);
            }

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

        #endregion

        #region Answer

        private static IAnswer GetAnswer(in DnStream Stream)
        {
            var Answer = new Answer()
            {
                Domain = GetDomain(in Stream),
            };

            if (Stream.Length < Stream.BytePosition + 2)
                throw new FormatException("Incomplete DNS Message, Missing Answer Record Type.");

            Answer.Type = Stream.ReadUShort().AsEnum<RecordType>();

            if (Answer.Type == RecordType.OPT)
            {
                Stream.Seek((ushort)(Stream.BytePosition - 3));

                return GetPseudoRecord(in Stream);
            }

            if (Stream.Length < Stream.BytePosition + 2)
                throw new FormatException("Incomplete DNS Message, Missing Answer Record Class.");

            Answer.Class = Stream.ReadUShort().AsEnum<RecordClass>();

            Answer.TimeToLive = Stream.ReadTimeSpan();

            Answer.Length = Stream.ReadUShort();

            if (Stream.Length < Answer.Length)
                throw new FormatException("Incomplete DNS Message, Missing Record Section.");

            Answer.Record = GetRecord(in Stream, Answer.Type);

            return Answer;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, IAnswer Answer)
        {
            if (Answer.Type == RecordType.OPT)
            {
                Set(in Stream, in Pointers, (PseudoRecord)Answer);

                return;
            }

            Set(in Stream, in Pointers, Answer.Domain);

            Stream.WriteUShort((byte)Answer.Type);

            if (Answer.Class != null)
                Stream.WriteUShort((byte)Answer.Class);

            if (Answer.TimeToLive != null)
                Stream.WriteTimeSpan((TimeSpan)Answer.TimeToLive);

            Stream.WriteUShort(Answer.Length);

            Set(in Stream, in Pointers, Answer.Record);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IAnswer Answer)
        {
            if (Answer.Type == RecordType.OPT)
            {
                return SizeOf((PseudoRecord)Answer);
            }

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
                RecordType.SRV => GetSRV(in Stream),
                RecordType.DNSKEY => GetDNSKEY(in Stream),
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
                        Set(in Stream, in Pointers, in MX);
                        break;
                    }
                case NS NS:
                    {
                        Set(in Stream, in Pointers, in NS);
                        break;
                    }
                case PTR PTR:
                    {
                        Set(in Stream, in Pointers, in PTR);
                        break;
                    }
                case SOA SOA:
                    {
                        Set(in Stream, in Pointers, in SOA);
                        break;
                    }
                case SRV SRV:
                    {
                        Set(in Stream, in Pointers, in SRV);
                        break;
                    }
                case DNSKEY DNSKEY:
                    {
                        Set(in Stream, in DNSKEY);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(Record), Record, null);
            }
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, IRecord Record)
        {
            return Record switch
            {
                A A => SizeOf(in A),
                CNAME CNAME => SizeOf(in Pointers, in CNAME),
                AAAA AAAA => SizeOf(in AAAA),
                TXT TXT => SizeOf(in TXT),
                MX MX => SizeOf(in Pointers, in MX),
                NS NS => SizeOf(in Pointers, in NS),
                PTR PTR => SizeOf(in Pointers, in PTR),
                SOA SOA => SizeOf(in Pointers, in SOA),
                SRV SRV => SizeOf(in Pointers, in SRV),
                DNSKEY DNSKEY => SizeOf(in DNSKEY),
                _ => throw new ArgumentOutOfRangeException(nameof(Record), Record, null)
            };
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
            return 16;
        }

        #endregion

        #region TXT

        private static ICharacterString GetCharacterString(in DnStream Stream)
        {
            var Length = Stream.ReadByte();

            return new CharacterString()
            {
                Length = Length,
                Value = Stream.ReadString(Length)
            };
        }

        private static void Set(in DnStream Stream, in ICharacterString CharacterString)
        {
            Stream.WriteByte(CharacterString.Length);
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
                Length = Stream.ReadByte(),
                Magic = Stream.ReadString(4),
                Version = Stream.ReadUShort().AsEnum<ESVersion>(),
                MinorVersion = Stream.ReadUShort(),
                Signature = Stream.ReadBytes(64).ToArray(),
                PublicKey = Stream.ReadBytes(32).ToArray(),
                ClientMagic = Stream.ReadBytes(8).ToArray(),
                Serial = Stream.ReadInt32(),
                StartTimeStamp = Stream.ReadEpoch(),
                EndTimeStamp = Stream.ReadEpoch(),
                //Extensions = Stream.ReadBytesToEnd().ToArray()
            };
        }

        private static void Set(in DnStream Stream, in ICertificate Certificate)
        {
            Stream.WriteByte(Certificate.Length);
            Stream.WriteString(Certificate.Magic);
            Stream.WriteUShort((ushort)Certificate.Version);
            Stream.WriteUShort(Certificate.MinorVersion);
            Stream.WriteBytes(Certificate.Signature);
            Stream.WriteBytes(Certificate.PublicKey);
            Stream.WriteBytes(Certificate.ClientMagic);
            Stream.WriteInt32(Certificate.Serial);
            Stream.WriteEpoch(Certificate.StartTimeStamp);
            Stream.WriteEpoch(Certificate.EndTimeStamp);
            //Stream.WriteBytes(Certificate.Extensions);
        }

        private static ushort SizeOf(in ICertificate Certificate)
        {
            //return (ushort)(126 + Certificate.Extensions.Length);
            return 125;
        }



        private static TXT GetTXT(in DnStream Stream)
        {
            var Length = Stream.ReadByte();

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

        public static ushort SizeOf(in TXT TXT)
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

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, in MX MX)
        {
            Stream.WriteShort(MX.Preference);
            Set(in Stream, in Pointers, MX.Exchange);
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

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, in NS NS)
        {
            Set(in Stream, in Pointers, NS.Domain);
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

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, in PTR PTR)
        {
            Set(in Stream, in Pointers, PTR.Domain);
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

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, in SOA SOA)
        {
            Set(in Stream, in Pointers, SOA.PrimaryNameServer);
            Set(in Stream, in Pointers, SOA.ResponsibleAuthorityMailbox);
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

        #region SRV

        private static SRV GetSRV(in DnStream Stream)
        {
            return new SRV()
            {
                Priority = Stream.ReadUShort(),
                Weight = Stream.ReadUShort(),
                Port = Stream.ReadUShort(),
                Target = GetDomain(in Stream)
            };
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, in SRV SRV)
        {
            Stream.WriteUShort(SRV.Priority);
            Stream.WriteUShort(SRV.Weight);
            Stream.WriteUShort(SRV.Port);
            Set(in Stream, in Pointers, SRV.Target);
        }

        private static ushort SizeOf(in SortedSet<string> Pointers, in SRV SRV)
        {
            return (ushort)(2 * 3 + SizeOf(in Pointers, SRV.Target));
        }

        #endregion

        #region DNSKEY

        private static Base64String GetBase64String(in DnStream Stream)
        {
            return new Base64String()
            {
                Bytes = Stream.ReadBytesToEnd().ToArray()
            };
        }

        private static DNSKEY GetDNSKEY(in DnStream Stream)
        {
            return new DNSKEY()
            {
                Flags = (ushort)(Stream.ReadByte() + Stream.ReadBits(7)),
                SecureEntryPoint = Stream.ReadBit().AsBool(),
                Algorithm = Stream.ReadByte().AsEnum<Algorithm>(),
                Protocol = Stream.ReadByte(),
                PublicKey = GetBase64String(in Stream)
            };
        }

        private static void Set(in DnStream Stream, in DNSKEY DNSKEY)
        {
            var Flags = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(Flags, DNSKEY.Flags);
            Stream.WriteByte(Flags[0]);
            Stream.WriteBits(7, Flags[1]);
            Stream.WriteBit(DNSKEY.SecureEntryPoint.AsByte());
            Stream.WriteByte((byte)DNSKEY.Algorithm);
            Stream.WriteByte(DNSKEY.Protocol);
            Stream.WriteBytes(DNSKEY.PublicKey.Bytes);
        }

        private static ushort SizeOf(in DNSKEY DNSKEY)
        {
            return (ushort)(4 + DNSKEY.PublicKey.Bytes.Length);
        }

        #endregion

        #region OPT

        private static PseudoRecord GetPseudoRecord(in DnStream Stream)
        {
            var PseudoRecord = new PseudoRecord()
            {
                Domain = GetDomain(in Stream),
                Type = Stream.ReadUShort().AsEnum<RecordType>(),
                Size = Stream.ReadUShort(),
                ExtendedType = ((ushort)Stream.ReadByte()).AsEnum<RecordType>(),
                Version = Stream.ReadByte(),
                DNSSEC = Stream.ReadBit().AsBool(),
                Zero = (ushort)(Stream.ReadBits(7) + Stream.ReadByte()),
                Length = Stream.ReadUShort()
            };

            if (PseudoRecord.Length != 0)
            {
                PseudoRecord.Data = Stream.ReadBytes(PseudoRecord.Length).ToArray();
            }

            return PseudoRecord;
        }

        private static void Set(in DnStream Stream, in Dictionary<string, ushort> Pointers, in PseudoRecord PseudoRecord)
        {
            Set(in Stream, in Pointers, PseudoRecord.Domain);
            Stream.WriteUShort((ushort)PseudoRecord.Type);
            Stream.WriteUShort(PseudoRecord.Size);
            Stream.WriteByte((byte)PseudoRecord.ExtendedType);
            Stream.WriteByte(PseudoRecord.Version);
            Stream.WriteBit(PseudoRecord.DNSSEC.AsByte());
            Stream.WriteBits(7, 0);
            Stream.WriteByte(0);
            Stream.WriteUShort(PseudoRecord.Length);

            if (PseudoRecord.Length != 0)
            {
                Stream.WriteBytes(PseudoRecord.Data);
            }
        }

        private static ushort SizeOf(in PseudoRecord PseudoRecord)
        {
            return (ushort)(11 + PseudoRecord.Length);
        }

        #endregion
    }
}

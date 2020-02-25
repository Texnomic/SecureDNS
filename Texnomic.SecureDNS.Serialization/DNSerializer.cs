using System;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using System.Collections.Generic;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Serialization.Extensions;

namespace Texnomic.SecureDNS.Serialization
{
    public static class DnSerializer
    {
        public static IMessage Deserialize(ref byte[] Raw)
        {
            ushort Index = 12;

            return new Message()
            {
                ID = Raw.GetUShort(0),
                MessageType = Raw[2].GetBit(0).AsEnum<MessageType>(),
                OperationCode = Raw[2].GetBits(1, 4).AsEnum<OperationCode>(),
                AuthoritativeAnswer = Raw[2].GetBit(5).AsEnum<AuthoritativeAnswer>(),
                Truncated = Raw[2].GetBit(6).AsBool(),
                RecursionDesired = Raw[2].GetBit(7).AsBool(),
                RecursionAvailable = Raw[3].GetBit(0).AsBool(),
                Zero = Raw[3].GetBit(1),
                AuthenticatedData = Raw[3].GetBit(2).AsBool(),
                CheckingDisabled = Raw[3].GetBit(3).AsBool(),
                ResponseCode = Raw[3].GetBits(4, 4).AsEnum<ResponseCode>(),
                QuestionsCount = Raw.GetUShort(4),
                AnswersCount = Raw.GetUShort(6),
                AuthorityCount = Raw.GetUShort(8),
                AdditionalCount = Raw.GetUShort(10),
                Questions = DeserializeQuestions(ref Raw, Raw.GetUShort(4), ref Index),
            };
        }

        public static IEnumerable<IQuestion> DeserializeQuestions(ref byte[] Raw, ushort Count, ref ushort Index)
        {
            var Questions = new List<IQuestion>();

            do
            {
                var Question = DeserializeQuestion(ref Raw, ref Index);

                Questions.Add(Question);
            }
            while (Questions.Count < Count);

            return Questions;
        }

        public static IQuestion DeserializeQuestion(ref byte[] Raw, ref ushort Index)
        {
            return new Question()
            {
                Domain = DeserializeDomain(ref Raw, ref Index),
                Type = Raw.GetUShort(Index += 1).AsEnum<RecordType>(),
                Class = Raw.GetUShort(Index += 2).AsEnum<RecordClass>()
            };
        }

        public static IDomain DeserializeDomain(ref byte[] Raw, ref ushort Index)
        {
            var Domain = new Domain()
            {
                Labels = DeserializeLabels(ref Raw, ref Index),
            };

            return Domain;
        }

        public static IEnumerable<string> DeserializeLabels(ref byte[] Raw, ref ushort Index)
        {
            var Labels = new List<string>();

            while (true)
            {
                var (Label, Type) = DeserializeLabel(ref Raw, Index);

                if (Label == null) 
                    break;

                Labels.Add(Label);

                switch (Type)
                {
                    case LabelType.Normal:
                        {
                            Index += (ushort)(Label.Length + 1);

                            break;
                        }
                    case LabelType.Compressed:
                        {
                            Index += 2;

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(LabelType));
                }
            }

            return Labels;
        }

        public static (string, LabelType) DeserializeLabel(ref byte[] Raw, ushort Index)
        {
            var LabelType = Raw[Index].GetBits(0, 2).AsEnum<LabelType>();

            switch (LabelType)
            {
                case LabelType.Normal:
                    {
                        var Length = Raw[Index].GetBits(2, 6);

                        if (Length == 0) return default;

                        var Label = Raw.GetString(++Index, Length);

                        return (Label, LabelType.Normal);
                    }
                case LabelType.Compressed:
                    {
                        var Pointer = Raw[Index].GetBits(2, 6) + Raw[++Index];

                        if (Pointer >= Index) throw new ArgumentOutOfRangeException(nameof(Pointer));

                        var (Label, _) = DeserializeLabel(ref Raw, (ushort)Pointer);

                        return (Label, LabelType.Compressed);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(LabelType));
            }
        }

        public static byte[] Serialize(IMessage Message)
        {
            return default;
        }
    }
}

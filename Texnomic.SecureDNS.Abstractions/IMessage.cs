using System.Collections.Generic;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Abstractions
{
    public interface IMessage
    {
        ushort ID { get; set; }

        MessageType MessageType { get; set; }

        OperationCode OperationCode { get; set; }

        AuthoritativeAnswer AuthoritativeAnswer { get; set; }

        bool Truncated { get; set; }

        bool RecursionDesired { get; set; }

        bool RecursionAvailable { get; set; }

        byte Zero { get; set; }

        bool AuthenticatedData { get; set; }

        bool CheckingDisabled { get; set; }

        ResponseCode ResponseCode { get; set; }

        ushort QuestionsCount { get; set; }

        IEnumerable<IQuestion> Questions { get; set; }

        ushort AnswersCount { get; set; }

        IEnumerable<IAnswer> Answers { get; set; }

        ushort AuthorityCount { get; set; }

        IEnumerable<IAnswer> Authority { get; set; }

        ushort AdditionalCount { get; set; }

        IEnumerable<IAnswer> Additional { get; set; }
    }
}

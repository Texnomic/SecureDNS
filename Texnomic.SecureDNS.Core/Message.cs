using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core;

public class Message : IMessage
{
    public Message()
    {
        Questions = Array.Empty<IQuestion>();
        Answers = Array.Empty<IAnswer>();
        Authority = Array.Empty<IAnswer>();
        Additional = Array.Empty<IAnswer>();
    }

    public ushort ID { get; set; }

    public MessageType MessageType { get; set; }

    public OperationCode OperationCode { get; set; }

    public AuthoritativeAnswer AuthoritativeAnswer { get; set; }

    public bool Truncated { get; set; }

    public bool RecursionDesired { get; set; }

    public bool RecursionAvailable { get; set; }

    public byte Zero { get; set; } = 0;

    public bool AuthenticatedData { get; set; }

    public bool CheckingDisabled { get; set; }

    public ResponseCode ResponseCode { get; set; }

    public ushort QuestionsCount { get; set; }

    public IEnumerable<IQuestion> Questions { get; set; }

    public ushort AnswersCount { get; set; }

    public IEnumerable<IAnswer> Answers { get; set; }

    public ushort AuthorityCount { get; set; }

    public IEnumerable<IAnswer> Authority { get; set; }

    public ushort AdditionalCount { get; set; }

    public IEnumerable<IAnswer> Additional { get; set; }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Abstractions
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

        int Zero { get; set; }

        bool AuthenticatedData { get; set; }

        bool CheckingDisabled { get; set; }

        ResponseCode ResponseCode { get; set; }


        ushort QuestionsCount { get; set; }


        ushort AnswersCount { get; set; }

        ushort AuthorityCount { get; set; }


        ushort AdditionalCount { get; set; }


        List<IQuestion> Questions { get; set; }


        List<IAnswer> Answers { get; set; }

        string Comment { get; set; }

        byte[] ToArray();
        Task<byte[]> ToArrayAsync();
        //IMessage FromArray(byte[] Data);
        //IMessage FromArray(ReadOnlySequence<byte> Data);
        //IMessage FromJson(string Json);
        string ToJson();
        Task<string> ToJsonAsync();
        
    }
}

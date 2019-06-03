using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Texnomic.DNS.Protocol.Models;

namespace Texnomic.SecureDNS
{
    public class Program
    {
        public static void Main(string[] Args)
        {
            Test();
            CreateHostBuilder(Args).Build().Run();
        }

        public static void Test()
        {
            //{Header={Id=39298, QuestionCount=1, AnswerRecordCount=0, AuthorityRecordCount=0, AdditionalRecordCount=0, Response=False, OperationCode=Query, AuthorativeServer=False, Truncated=False, RecursionDesired=True, RecursionAvailable=False, AuthenticData=False, CheckingDisabled=False, ResponseCode=NoError}, Questions=[{Name=facebook.com, Type=A, Class=IN}], AdditionalRecords=[]}
            var RequestBytes = Convert.FromBase64String("mYIBAAABAAAAAAAACGZhY2Vib29rA2NvbQAAAQAB");

            //{Header={Id=39298, QuestionCount=1, AnswerRecordCount=0, AuthorityRecordCount=0, AdditionalRecordCount=0, Response=False, OperationCode=Query, AuthorativeServer=False, Truncated=False, RecursionDesired=True, RecursionAvailable=False, AuthenticData=False, CheckingDisabled=False, ResponseCode=NoError}, Questions=[{Name=facebook.com, Type=A, Class=IN}], AdditionalRecords=[]} => {Header={Id=39298, QuestionCount=1, AnswerRecordCount=1, AuthorityRecordCount=0, AdditionalRecordCount=0, Response=True, OperationCode=Query, AuthorativeServer=False, Truncated=False, RecursionDesired=True, RecursionAvailable=True, AuthenticData=False, CheckingDisabled=False, ResponseCode=NoError}, Questions=[{Name=facebook.com, Type=A, Class=IN}], AnswerRecords=[{Name=facebook.com, Type=A, Class=IN, TimeToLive=00:03:10, DataLength=4, IPAddress=179.60.192.36}], AuthorityRecords=[], AdditionalRecords=[]}
            var ResponseBytes = Convert.FromBase64String("mYKBgAABAAEAAAAACGZhY2Vib29rA2NvbQAAAQABCGZhY2Vib29rA2NvbQAAAQABAAAAvgAEszzAJA==");

            var Message = new Message(RequestBytes);

            var ID = Message.RecordIndexes;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}

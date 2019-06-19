using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using Texnomic.ORMi;
using Texnomic.WMI.Network;

namespace Texnomic.SecureDNS.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        public void Test()
        {
            var Wmi = new WmiContext();

            var NICs = Wmi.Query<NetworkAdapterConfiguration>();

            var NIC = NICs.Where(Int => Int.Index == 9).First();

            var Reult = NIC.SetDnsServers(IPAddress.Parse("127.0.0.1"));

            //var Resolver = new TlsRequestResolver(new IPEndPoint(IPAddress.Parse("1.1.1.1"), 853));

            //var Server = new DnsServer(Resolver);

            //Server.Listen();

            //{Header={Id=39298, QuestionCount=1, AnswerRecordCount=0, AuthorityRecordCount=0, AdditionalRecordCount=0, Response=False, OperationCode=Query, AuthorativeServer=False, Truncated=False, RecursionDesired=True, RecursionAvailable=False, AuthenticData=False, CheckingDisabled=False, ResponseCode=NoError}, Questions=[{Name=facebook.com, Type=A, Class=IN}], AdditionalRecords=[]}
            //var RequestBytes = Convert.FromBase64String("mYIBAAABAAAAAAAACGZhY2Vib29rA2NvbQAAAQAB");

            //{Header={Id=39298, QuestionCount=1, AnswerRecordCount=0, AuthorityRecordCount=0, AdditionalRecordCount=0, Response=False, OperationCode=Query, AuthorativeServer=False, Truncated=False, RecursionDesired=True, RecursionAvailable=False, AuthenticData=False, CheckingDisabled=False, ResponseCode=NoError}, Questions=[{Name=facebook.com, Type=A, Class=IN}], AdditionalRecords=[]} => {Header={Id=39298, QuestionCount=1, AnswerRecordCount=1, AuthorityRecordCount=0, AdditionalRecordCount=0, Response=True, OperationCode=Query, AuthorativeServer=False, Truncated=False, RecursionDesired=True, RecursionAvailable=True, AuthenticData=False, CheckingDisabled=False, ResponseCode=NoError}, Questions=[{Name=facebook.com, Type=A, Class=IN}], AnswerRecords=[{Name=facebook.com, Type=A, Class=IN, TimeToLive=00:03:10, DataLength=4, IPAddress=179.60.192.36}], AuthorityRecords=[], AdditionalRecords=[]}
            //var ResponseBytes = Convert.FromBase64String("mYKBgAABAAEAAAAACGZhY2Vib29rA2NvbQAAAQABCGZhY2Vib29rA2NvbQAAAQABAAAAvgAEszzAJA==");

            //var Message = new Message(ResponseBytes);

            //var ID = Message.Answers;
        }
    }
}

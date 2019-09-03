using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;

namespace Texnomic.DNS.Tests.Resolvers
{
    [TestClass]
    public class DoT
    {
        private ushort ID;
        private IResolver Resolver;
        private Message RequestMessage;
        private Message ResponseMessage;

        [TestInitialize]
        public void Initialize()
        {
            Resolver = new TLS(IPAddress.Parse("8.8.4.4"),
                "3082010A02820101009D1FA4EF5D3E883319ABE79A6DC82BF72A3C1D312EAD5DAB4143368F042D45FA819BC8DD1E3F0227A2A2398DB945E0AB3F1AB143A87F83884BFCCB40230DAA673C2A441ECADF392ABBDB7CA3677DCE04BB92480BBC5F64AACA1A5BB295A66A0EDCE7060F05B88BCA08D0AB36290B192815504E58972D60AFF9F7EC8DAB488107E32E2F89B0DBCDF5CE022530EB32D1740826FB7584A81225BACC70005B74453EADCF3EE2A0AE75E9B3D85A3416A10D7670CD1FFE35D43C03A371DDFBE4F687BCF902DECB68552242F0211FA9572E61D3BBBFEEE782219AE5793608EE06800B347D6340732521EEB813AB544BB603148CEAF7C1AAA62EF4DA2AC23415052C16B90203010001");

            ID = (ushort) new Random().Next();

            RequestMessage = new Message()
            {
                ID = ID,
                //MessageType = MessageType.Query,
                //OperationCode = OperationCode.Query,
                //AuthoritativeAnswer = AuthoritativeAnswer.Cache,
                //Truncated = false,
                RecursionDesired = true,
                //RecursionAvailable = false,
                //Zero = 0,
                //AuthenticatedData = false,
                //CheckingDisabled = false,
                //ResponseCode = ResponseCode.NoError,
                //QuestionsCount = 1,
                //AnswersCount = 0,
                Questions = new[]
                {
                    new Question()
                    {
                        Domain = Domain.FromString("facebook.com"),
                        Class = RecordClass.Internet,
                        Type = RecordType.A
                    }
                }
            };
        }

        [TestMethod]
        public async Task MessageAsync()
        {
            var Msg = await Resolver.ResolveAsync(RequestMessage);

            //Assert.AreEqual(Msg.ID, 39298);
            Assert.AreEqual(Msg.AnswersCount, 1);
            Assert.AreEqual(Msg.QuestionsCount, 1);
            Assert.AreEqual(Msg.AuthorityCount, 0);
            Assert.AreEqual(Msg.AdditionalCount, 0);
            Assert.AreEqual(Msg.Truncated, false);
            Assert.AreEqual(Msg.RecursionAvailable, false);
            Assert.AreEqual(Msg.RecursionDesired, Enums.RecursionDesired.Recursive);
            Assert.AreEqual(Msg.AuthoritativeAnswer, Enums.AuthoritativeAnswer.Cache);
            Assert.AreEqual(Msg.ResponseCode, Enums.ResponseCode.NoError);
            Assert.AreEqual(Msg.OperationCode, Enums.OperationCode.Query);

            var Answer = Msg.Answers?.First();

            Assert.IsNotNull(Answer);
            Assert.AreEqual(Answer.Type, Enums.RecordType.A);
            Assert.AreEqual(Answer.Class, Enums.RecordClass.Internet);
            Assert.AreEqual(Answer.Domain.ToString(), "facebook.com");
        }

        [TestMethod]
        public async Task BytesAsync()
        {
            var Bytes = await Resolver.ResolveAsync(RequestMessage.ToArray());

            var Msg = Message.FromArray(Bytes);

            //Assert.AreEqual(Msg.ID, 39298);
            Assert.AreEqual(Msg.AnswersCount, 1);
            Assert.AreEqual(Msg.QuestionsCount, 1);
            Assert.AreEqual(Msg.AuthorityCount, 0);
            Assert.AreEqual(Msg.AdditionalCount, 0);
            Assert.AreEqual(Msg.Truncated, false);
            Assert.AreEqual(Msg.RecursionAvailable, false);
            Assert.AreEqual(Msg.RecursionDesired, Enums.RecursionDesired.Recursive);
            Assert.AreEqual(Msg.AuthoritativeAnswer, Enums.AuthoritativeAnswer.Cache);
            Assert.AreEqual(Msg.ResponseCode, Enums.ResponseCode.NoError);
            Assert.AreEqual(Msg.OperationCode, Enums.OperationCode.Query);

            var Answer = Msg.Answers?.First();

            Assert.IsNotNull(Answer);
            Assert.AreEqual(Answer.Type, Enums.RecordType.A);
            Assert.AreEqual(Answer.Class, Enums.RecordClass.Internet);
            Assert.AreEqual(Answer.Domain.ToString(), "facebook.com");
        }
    }
}

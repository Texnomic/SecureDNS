using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Tests.Providers.HTTPS
{
    [TestClass]
    public class Texnomic
    {
        private ushort ID;
        private IProtocol Resolver;
        private IMessage RequestMessage;
        private IMessage ResponseMessage;


        [TestInitialize]
        public void Initialize()
        {
            //ID = (ushort) new Random().Next();

            //Resolver = new HTTPs(new Uri("https://sdns.azurewebsites.net"), "3082010A0282010100A73EA5C80EE11262AA736770C8B170AEE84F815FDB20DF90277A540BC886AB75674F8EEC91852EEC0A8695E12139538266117B21290694E0E7B5C1CC702A949D56D685B7A496E3F6E34185D30E03642E3D85C6596C76C03C0BFF21C0EA9F006F6936A267F97E7C722489EEF6016A9FB409FC229E4E3BBB92635398F3A98C38109F2C43574206D43F8612E7CD59F33484B6C8E788F3BEFBF3B0C1BC802BB8F7C9F2F0D2B954284622A040C493A7CC7FEB9F2FA4E2A3C94537397BF1C79E668CFEC7D45EEB05A4A8254C8F1F5B29E72B324100112BE2F9260CB6985FC57B2BB7F54E8F6C0923B56150B5906EAB9D576443F79C7C3245E9193F544A833F765949AB0203010001");

            //RequestMessage = new Message()
            //{
            //    ID = ID,
            //    RecursionDesired = true,
            //    Questions = new List<IQuestion>()
            //    {
            //        new Question()
            //        {
            //            Domain = Domain.FromString("facebook.com"),
            //            Class = RecordClass.Internet,
            //            Type = RecordType.A
            //        }
            //    }
            //};
        }

        [TestMethod]
        public async Task QueryAsync()
        {
            //Using Binary Format Over HTTPs 

            var RequestBytes = await RequestMessage.ToArrayAsync();

            var ResponseBytes = await Resolver.ResolveAsync(RequestBytes);

            ResponseMessage = await Message.FromArrayAsync(ResponseBytes);

            Assert.AreEqual(ID, ResponseMessage.ID);
            Assert.IsNotNull(ResponseMessage.Questions);
            Assert.IsNotNull(ResponseMessage.Answers);
            Assert.IsInstanceOfType(ResponseMessage.Answers.First().Record, typeof(DNS.Records.A));
        }
    }
}

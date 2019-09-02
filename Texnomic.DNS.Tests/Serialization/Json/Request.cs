using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Enums;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Tests.Serialization.Json
{
    [TestClass]
    public class Request
    {
        private const string RequestJson =
            "{\"Status\": 0,\"TC\": false,\"RD\": true, \"RA\": true, \"AD\": true,\"CD\": false,\"Question\":[{\"name\": \"example.com.\", \"type\": 1}]}";

        private const string ResponseJson =
            "{\"Status\": 0,\"TC\": false,\"RD\": true, \"RA\": true, \"AD\": true,\"CD\": false,\"Question\":[{\"name\": \"example.com.\", \"type\": 1}],\"Answer\":[{\"name\": \"example.com.\", \"type\": 1, \"TTL\": 10392, \"data\": \"93.184.216.34\"}]}";



        [TestMethod]
        public void FromJson()
        {
            var RequestMsg = Message.FromJson(RequestJson);
            var ResponseMsg = Message.FromJson(ResponseJson);
        }

        [TestMethod]
        public void ToJson()
        {
            var Msg = new Message()
            {
                ResponseCode = ResponseCode.NoError,
                Truncated = false,
                RecursionDesired = true,
                RecursionAvailable = true,
                AuthenticatedData = true,
                CheckingDisabled = false,
                Questions = new[]
                {
                    new Question()
                    {
                        Domain = Domain.FromString("example.com")
                    }
                }
            };

            var Json = Msg.ToJson();
        }
    }
}

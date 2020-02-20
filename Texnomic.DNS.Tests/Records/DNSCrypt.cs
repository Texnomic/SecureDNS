using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Options;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Records;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Tests.Records
{
    [TestClass]
    public class DNSCrypt
    {
        [TestMethod]
        public async Task QueryAsync()
        {
            var TXT = new TXT()
            {
                Length = (byte)"Texnomic".Length,
                Text = "Texnomic"
            };

            var BS = new BinarySerializer();

            var Test = await BS.SerializeAsync(TXT);
        }
    }
}

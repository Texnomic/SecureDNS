using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.ENS.FIFSRegistrar.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Records;
using Texnomic.ENS.PublicResolver.ContractDefinition;
using PublicResolverService = Texnomic.ENS.PublicResolver.PublicResolverService;

namespace Texnomic.ENS.Infrastructure.Tests
{
    [TestClass]
    public class CairoSecurityCamp
    {
        private Web3 Web3;
        private EnsUtil EnsUtil;
        private ENSRegistryService ENSRegistryService;
        private FIFSRegistrarService FIFSRegistrarService;
        private PublicResolverService PublicResolverService;

        private const string Domain = "dina";

        private const string OwnerAddress = "0xCf5f18513388Ad45f5C28e7c965e5aB81CA5C1F3";

        private const string ENSRegistryAddress = "0x7A5a999E06E3Ee43066D9Ea1725e0f46Ec2cf557";
        private const string FIFSRegistrarAddress = "0x779A44526DA54a033c5Db71D04c072AE47AB5BDE";
        private const string PublicResolverAddress = "0x5e7270094CfDacdAd113256e2f0b6016c5772757";



        [TestInitialize]
        public void Initialize()
        {
            Web3 = new Web3($"http://localhost:7545");
            EnsUtil = new EnsUtil();
            ENSRegistryService = new ENSRegistryService(Web3, ENSRegistryAddress);
            FIFSRegistrarService = new FIFSRegistrarService(Web3, FIFSRegistrarAddress);
            PublicResolverService = new PublicResolverService(Web3, PublicResolverAddress);
        }

        [TestMethod]
        public async Task Demo()
        {
            var RegistrarTransactionReceipt = await Register();
            var SetResolverTransactionReceipt = await SetResolver();
            var SetAddressTransactionReceipt = await SetAddress();
            var SetARecordTransactionReceipt = await SetARecord();
            var SetTextTransactionReceipt = await SetTxtRecord();

            var Address = await QueryAddress();
            var Value = await GetTxtRecord();
            var HasARecord = await HasRecord();
            var ARecord = await GetARecord();
        }


        public async Task<TransactionReceipt> Register()
        {
            var RegisterRequest = new RegisterFunction()
            {
                Subnode = EnsUtil.GetLabelHash(Domain).HexToByteArray(),
                Owner = OwnerAddress,
                FromAddress = OwnerAddress,
                Gas = new HexBigInteger(100000),
                GasPrice = new HexBigInteger(100000),
            };

            return await FIFSRegistrarService.RegisterRequestAndWaitForReceiptAsync(RegisterRequest);
        }

        public async Task<TransactionReceipt> SetResolver()
        {
            var SetResolverFunction = new SetResolverFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Resolver = PublicResolverAddress,
                FromAddress = OwnerAddress,
                Gas = new HexBigInteger(100000),
                GasPrice = new HexBigInteger(100000),
            };

            return await ENSRegistryService.SetResolverRequestAndWaitForReceiptAsync(SetResolverFunction);
        }


        public async Task<TransactionReceipt> SetAddress()
        {
            var SetAddressFunction = new SetAddrFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Addr = OwnerAddress,
                FromAddress = OwnerAddress,
                Gas = new HexBigInteger(100000),
                GasPrice = new HexBigInteger(100000),
            };

            return await PublicResolverService.SetAddrRequestAndWaitForReceiptAsync(SetAddressFunction);
        }


        public async Task<string> QueryAddress()
        {
            var AddressFunction = new AddrFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
            };

            return await PublicResolverService.AddrQueryAsync(AddressFunction);
        }


        public async Task<TransactionReceipt> SetARecord()
        {
            var Answer = new Answer()
            {
                Domain = new Domain($"{Domain}.eth"),

                Type = RecordType.A,

                Class = RecordClass.Internet,

                TimeToLive = new TimeToLive()
                {
                    Value = new TimeSpan(1, 0, 0, 0)
                },

                Record = new A()
                {
                    Address = new IPv4Address()
                    {
                        Value = IPAddress.Parse("127.0.0.1")
                    }
                }
            };

            var SetDNSRecordsFunction = new SetDNSRecordsFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Data = Answer.ToArray(),
                FromAddress = OwnerAddress,
                Gas = new HexBigInteger(100000),
                GasPrice = new HexBigInteger(100000),
            };

            return await PublicResolverService.SetDNSRecordsRequestAndWaitForReceiptAsync(SetDNSRecordsFunction);
        }

        public async Task<TransactionReceipt> SetTxtRecord()
        {
            var SetTextFunction = new SetTextFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Key = "MyKey",
                Value = "MyValue",
                FromAddress = OwnerAddress
            };

            return await PublicResolverService.SetTextRequestAndWaitForReceiptAsync(SetTextFunction);
        }


        public async Task<string> GetTxtRecord()
        {
            var TextFunction = new TextFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Key = "MyKey"
            };

            return await PublicResolverService.TextQueryAsync(TextFunction);
        }


        public async Task<bool> HasRecord()
        {
            var Name = Encoding.ASCII.GetString(new Domain($"{Domain}.eth").ToArray());

            var HasDNSRecordsFunction = new HasDNSRecordsFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Name = EnsUtil.GetLabelHash(Name).HexToByteArray(),
            };

            return await PublicResolverService.HasDNSRecordsQueryAsync(HasDNSRecordsFunction);
        }


        public async Task<Answer> GetARecord()
        {
            var Name = Encoding.ASCII.GetString(new Domain($"{Domain}.eth").ToArray());

            var DNSRecordFunction = new DnsRecordFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Name = EnsUtil.GetLabelHash(Name).HexToByteArray(),
                Resource = 1
            };

            var Bytes = await PublicResolverService.DnsRecordQueryAsync(DNSRecordFunction);

            return await Answer.FromArrayAsync(Bytes);
        }
    }
}

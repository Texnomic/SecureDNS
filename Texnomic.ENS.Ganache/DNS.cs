using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.ABI.Util;
using Nethereum.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.ENS.FIFSRegistrar.ContractDefinition;
using Nethereum.ENS.PublicResolver.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Records;

namespace Texnomic.ENS.Ganache
{
    [TestClass]
    public class DNS
    {
        private Web3 Web3;
        private EnsUtil EnsUtil;
        private ENSRegistryService ENSRegistryService;
        private FIFSRegistrarService FIFSRegistrarService;
        private PublicResolverService PublicResolverService;
        private const string ENSRegistryAddress = "0x17682BA6c6104FBCF2b6A82ADaB00C96282F86dc";
        private const string FIFSRegistrarAddress = "0x5017A8983684877e2D056d39eeD6462D01F63500";
        private const string OwnerAddress = "0x7f79721eE7457dD09664C2BF91403A540FE78be4";
        private const string PublicResolverAddress = "0xA9565F89BBC5f426Eae1ab6641A0816a9b55583a";
        private const string PublicResolverABI =
            "[{\"constant\":true,\"inputs\":[{\"name\":\"interfaceID\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"pure\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"setDNSRecords\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"key\",\"type\":\"string\"},{\"name\":\"value\",\"type\":\"string\"}],\"name\":\"setText\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"interfaceID\",\"type\":\"bytes4\"}],\"name\":\"interfaceImplementer\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"contentTypes\",\"type\":\"uint256\"}],\"name\":\"ABI\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"},{\"name\":\"\",\"type\":\"bytes\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"x\",\"type\":\"bytes32\"},{\"name\":\"y\",\"type\":\"bytes32\"}],\"name\":\"setPubkey\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"hash\",\"type\":\"bytes\"}],\"name\":\"setContenthash\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"}],\"name\":\"addr\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"name\",\"type\":\"bytes32\"}],\"name\":\"hasDNSRecords\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"key\",\"type\":\"string\"}],\"name\":\"text\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"contentType\",\"type\":\"uint256\"},{\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"setABI\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"}],\"name\":\"name\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"name\",\"type\":\"string\"}],\"name\":\"setName\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"name\",\"type\":\"bytes32\"},{\"name\":\"resource\",\"type\":\"uint16\"}],\"name\":\"dnsRecord\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"}],\"name\":\"clearDNSZone\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"}],\"name\":\"contenthash\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"}],\"name\":\"pubkey\",\"outputs\":[{\"name\":\"x\",\"type\":\"bytes32\"},{\"name\":\"y\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"addr\",\"type\":\"address\"}],\"name\":\"setAddr\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"interfaceID\",\"type\":\"bytes4\"},{\"name\":\"implementer\",\"type\":\"address\"}],\"name\":\"setInterface\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"\",\"type\":\"bytes32\"},{\"name\":\"\",\"type\":\"address\"},{\"name\":\"\",\"type\":\"address\"}],\"name\":\"authorisations\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"name\":\"_ens\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":true,\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"target\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"isAuthorised\",\"type\":\"bool\"}],\"name\":\"AuthorisationChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":true,\"name\":\"indexedKey\",\"type\":\"string\"},{\"indexed\":false,\"name\":\"key\",\"type\":\"string\"}],\"name\":\"TextChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"x\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"y\",\"type\":\"bytes32\"}],\"name\":\"PubkeyChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"name\",\"type\":\"string\"}],\"name\":\"NameChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":true,\"name\":\"interfaceID\",\"type\":\"bytes4\"},{\"indexed\":false,\"name\":\"implementer\",\"type\":\"address\"}],\"name\":\"InterfaceChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"name\",\"type\":\"bytes\"},{\"indexed\":false,\"name\":\"resource\",\"type\":\"uint16\"},{\"indexed\":false,\"name\":\"record\",\"type\":\"bytes\"}],\"name\":\"DNSRecordChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"name\",\"type\":\"bytes\"},{\"indexed\":false,\"name\":\"resource\",\"type\":\"uint16\"}],\"name\":\"DNSRecordDeleted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"}],\"name\":\"DNSZoneCleared\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"hash\",\"type\":\"bytes\"}],\"name\":\"ContenthashChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"a\",\"type\":\"address\"}],\"name\":\"AddrChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"node\",\"type\":\"bytes32\"},{\"indexed\":true,\"name\":\"contentType\",\"type\":\"uint256\"}],\"name\":\"ABIChanged\",\"type\":\"event\"},{\"constant\":false,\"inputs\":[{\"name\":\"node\",\"type\":\"bytes32\"},{\"name\":\"target\",\"type\":\"address\"},{\"name\":\"isAuthorised\",\"type\":\"bool\"}],\"name\":\"setAuthorisation\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

        private const string Domain = "ehab";

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
        public async Task Register()
        {
            var RegisterRequest = new RegisterFunction()
            {
                Subnode = EnsUtil.GetLabelHash(Domain).HexToByteArray(),
                Owner = OwnerAddress,
                FromAddress = OwnerAddress
            };

            var RegistrarTransactionReceipt = await FIFSRegistrarService.RegisterRequestAndWaitForReceiptAsync(RegisterRequest);
        }

        [TestMethod]
        public async Task SetResolver()
        {
            var SetResolverFunction = new SetResolverFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Resolver = PublicResolverAddress,
                FromAddress = OwnerAddress
            };

            var SetResolverTransactionReceipt = await ENSRegistryService.SetResolverRequestAndWaitForReceiptAsync(SetResolverFunction);
        }

        [TestMethod]
        public async Task SetAddress()
        {
            var SetAddressFunction = new SetAddrFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                Addr = OwnerAddress,
                FromAddress = OwnerAddress
            };

            var SetAddressTransactionReceipt = await PublicResolverService.SetAddrRequestAndWaitForReceiptAsync(SetAddressFunction);
        }

        [TestMethod]
        public async Task QueryAddress()
        {
            var AddressFunction = new AddrFunction()
            {
                Node = EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
            };

            var Address = await PublicResolverService.AddrQueryAsync(AddressFunction);
        }

        [TestMethod]
        public async Task SetARecord()
        {
            var Contract = Web3.Eth.GetContract(PublicResolverABI, PublicResolverAddress);

            var Function = Contract.GetFunction("setDNSRecords");

            var ARecord = new Answer()
            {
                Domain = new Domain($"{Domain}.eth"),

                Type = RecordType.A,

                Class = RecordClass.Internet,

                TimeToLive = new TimeToLive()
                {
                    Value = new TimeSpan(1)
                },

                Record = new A()
                {
                    Address = new IPv4Address()
                    {
                        Value = IPAddress.Parse("127.0.0.1")
                    }
                }
            };

            // Node == LabelHash
            // Name == NameHash
            var Parameters = new object[]
            {
                EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                ARecord.ToArray()
            };

            var Data = Function.GetData(Parameters);

            var TransactionInput = new TransactionInput()
            {
                To = PublicResolverAddress,
                From = OwnerAddress,
                Data = Data,
                Gas = new HexBigInteger(100000),
                GasPrice = new HexBigInteger(100000),
            };

            var Result =  await Web3.Eth.TransactionManager.SendTransactionAndWaitForReceiptAsync(TransactionInput,
                new CancellationTokenSource());
        }

        [TestMethod]
        public async Task SetTxtRecord()
        {
            var Contract = Web3.Eth.GetContract(PublicResolverABI, PublicResolverAddress);

            var Function = Contract.GetFunction("setText");

            var Parameters = new object[]
            {
                EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                "MyKey",
                "MyValue"
            };

            var Data = Function.GetData(Parameters);

            var TransactionInput = new TransactionInput()
            {
                To = PublicResolverAddress,
                From = OwnerAddress,
                Data = Data,
                Gas = new HexBigInteger(100000),
                GasPrice = new HexBigInteger(100000),
            };

            var Result = await Web3.Eth.TransactionManager.SendTransactionAndWaitForReceiptAsync(TransactionInput,
                new CancellationTokenSource());
        }

        [TestMethod]
        public async Task GetTxtRecord()
        {
            var Contract = Web3.Eth.GetContract(PublicResolverABI, PublicResolverAddress);

            var Function = Contract.GetFunction("text");

            var Parameters = new object[]
            {
                EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                "MyKey"
            };

            var Result = await Function.CallAsync<string>(Parameters);
        }

        [TestMethod]
        public async Task HasRecord()
        {
            var Contract = Web3.Eth.GetContract(PublicResolverABI, PublicResolverAddress);

            var Function = Contract.GetFunction("hasDNSRecords");

            var Test = "0x0574657374310365746800".HexToUTF8String();

            var Parameters = new object[]
            {
                EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                EnsUtil.GetLabelHash($"\u0005{Domain}\u0003eth\0").HexToByteArray(),
            };

            var Result = await Function.CallAsync<bool>(Parameters);
        }

        [TestMethod]
        public async Task GetRecord()
        {
            var Contract = Web3.Eth.GetContract(PublicResolverABI, PublicResolverAddress);

            var Function = Contract.GetFunction("dnsRecord");

            var Parameters = new object[]
            {
                EnsUtil.GetNameHash($"{Domain}.eth").HexToByteArray(),
                EnsUtil.GetLabelHash($"\u0004{Domain}\u0003eth\0").HexToByteArray(),
                1
            };

            var Result = await Function.CallAsync<byte[]>(Parameters);
        }
    }
}

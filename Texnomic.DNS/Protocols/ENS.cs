using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySerialization;
using Nethereum.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Texnomic.DNS.Models;
using Nethereum.Web3;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Records;
using Texnomic.ENS.BaseRegistrar;
using Texnomic.ENS.BaseRegistrar.ContractDefinition;

using OwnerFunction = Nethereum.ENS.ENSRegistry.ContractDefinition.OwnerFunction;

namespace Texnomic.DNS.Protocols
{
    public class ENS : IProtocol
    {
        private readonly Web3 Web3;
        private readonly EnsUtil EnsUtil;
        private readonly ENSRegistryService ENSRegistryService;
        private readonly BaseRegistrarService BaseRegistrarService;
        private readonly BinarySerializer BinarySerializer;
        public const string BaseRegistrar = "0xFaC7BEA255a6990f749363002136aF6556b31e04";
        public const string MainnetRegistryAddress = "0x314159265dd8dbb310642f98f50c066173c1259b";
        public const string RopstenRegistryAddress = "0x112234455c3a32fd11230c42e7bccd4a84e02010";
        public const string RinkebyRegistryAddress = "0xe7410170f87102df0055eb195163a03b7f2bff4a";
        public const string GoerliRegistryAddress = "0x112234455c3a32fd11230c42e7bccd4a84e02010";

        public ENS(Uri Web3Uri, string RegistryAddress)
        {
            Web3 = new Web3(Web3Uri.ToString());
            EnsUtil = new EnsUtil();
            ENSRegistryService = new ENSRegistryService(Web3, RegistryAddress);
            BaseRegistrarService = new BaseRegistrarService(Web3, BaseRegistrar);
            BinarySerializer = new BinarySerializer();
        }

        public async ValueTask<(string Registrant, ulong TimeToLive, string Resolver, string Contract)> ResolveAsync(string Domain)
        {
            if (!Domain.EndsWith(".eth", StringComparison.InvariantCultureIgnoreCase))
                throw new NotImplementedException($"Only .ETH Top-Level Domain is Supported.");

            var NameHashString = EnsUtil.GetNameHash(Domain);

            var NameHashBytes = NameHashString.HexToByteArray();

            var TimeToLive = await GetTimeToLiveAsync(NameHashBytes);

            var ResolverFunction = new ResolverFunction()
            {
                Node = NameHashBytes
            };

            //get the resolver address from ENS
            var Resolver = await ENSRegistryService.ResolverQueryAsync(ResolverFunction);

            var OwnerFunction = new OwnerFunction()
            {
                Node = NameHashBytes
            };

            var Registrant = await ENSRegistryService.OwnerQueryAsync(OwnerFunction);

            var ResolverService = new PublicResolverService(Web3, Resolver);

            //and get the address from the resolver
            var Contract = await ResolverService.AddrQueryAsync(NameHashBytes);

            return (Registrant, TimeToLive, Resolver, Contract);
        }

        public async ValueTask<DateTime> GetDomainExpiryAsync(string Domain)
        {
            var Label = Domain.Split('.')[0];

            var LabelHash = EnsUtil.GetLabelHash(Label);

            var LabelHashBigInteger = LabelHash.HexToBigInteger(false);

            var NameExpireFunction = new NameExpiresFunction()
            {
                Id = LabelHashBigInteger
            };

            var Epoch = await BaseRegistrarService.NameExpiresQueryAsync(NameExpireFunction);

            var DateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((long)Epoch);

            return DateTime;
        }


        private async Task<ulong> GetTimeToLiveAsync(byte[] NameHashBytes)
        {
            var TtlFunction = new TtlFunction()
            {
                Node = NameHashBytes
            };

            return await ENSRegistryService.TtlQueryAsync(TtlFunction);
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public IMessage Resolve(IMessage Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            var Request = await BinarySerializer.DeserializeAsync<Message>(Query);

            var Response = await ResolveAsync(Request);

            return await BinarySerializer.SerializeAsync(Response);
        }

        public async Task<IMessage> ResolveAsync(IMessage Query)
        {
            var (Registrant, TimeToLive, Resolver, Contract) = await ResolveAsync(Query.Questions.First().Name);

            Query.MessageType = MessageType.Response;

            Query.Answers = new List<IAnswer>()
            {
                new Answer()
                {
                    TimeToLive = new TimeToLive()
                    {
                        Value = TimeSpan.FromSeconds(TimeToLive)
                    },

                    Length = (ushort)(Resolver.Length + Registrant.Length),

                    Domain = (Domain)Query.Questions.First().Domain,

                    Record = new ETH()
                    {
                        Resolver = Resolver,
                        Registrant = Registrant,
                        Contract = Contract
                    },
                }
            };

            return Query;
        }

        private bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {

            }

            IsDisposed = true;
        }

        ~ENS()
        {
            Dispose(false);
        }
    }
}

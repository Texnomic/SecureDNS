using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Texnomic.DNS.Models;
using Nethereum.Web3;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Records;

namespace Texnomic.DNS.Resolvers
{
    public class ENS : IResolver
    {
        private readonly Web3 Web3;
        private readonly EnsUtil EnsUtil;
        private readonly ENSRegistryService ENSRegistryService;
        private const string Mainnet = "0x314159265dd8dbb310642f98f50c066173c1259b";
        private const string Ropsten = "0x112234455c3a32fd11230c42e7bccd4a84e02010";
        private const string Rinkeby = "0xe7410170f87102df0055eb195163a03b7f2bff4a";
        private const string Goerli = "0x112234455c3a32fd11230c42e7bccd4a84e02010";

        public ENS(string ProjectID)
        {
            Web3 = new Web3($"https://mainnet.infura.io/v3/{ProjectID}");
            EnsUtil = new EnsUtil();
            ENSRegistryService = new ENSRegistryService(Web3, Mainnet);
        }

        public async Task<string> ResolveAsync(string Domain)
        {
            var NameHashString = EnsUtil.GetNameHash(Domain);

            var NameHashBytes = NameHashString.HexToByteArray();

            var ResolverFunction = new ResolverFunction()
            {
                Node = NameHashBytes
            };

            //get the resolver address from ENS
            var ResolverAddress = await ENSRegistryService.ResolverQueryAsync(ResolverFunction);

            var ResolverService = new PublicResolverService(Web3, ResolverAddress);

            //and get the address from the resolver
            var Address = await ResolverService.AddrQueryAsync(NameHashBytes);

            return Address;
        }

        public string Resolve(string Domain)
        {
            return Async.RunSync(() => ResolveAsync(Domain));
        }

        public byte[] Resolve(byte[] Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public Message Resolve(Message Query)
        {
            return Async.RunSync(() => ResolveAsync(Query));
        }

        public async Task<byte[]> ResolveAsync(byte[] Query)
        {
            var Request = Message.FromArray(Query);

            var Response = await ResolveAsync(Request);

            return Response.ToArray();
        }

        public async Task<Message> ResolveAsync(Message Query)
        {
            var Address = await ResolveAsync(Query.Questions.First().Name);

            Query.MessageType = MessageType.Response;
            Query.Answers = new List<Answer>()
            {
                new Answer()
                {
                    TimeToLive = new TimeToLive() { Value = new TimeSpan(0,0,60 * 60) },
                    Length = (ushort)Address.Length,
                    Domain = Query.Questions.First().Domain,
                    Record = new ETH()
                    {
                        Address = Address,
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

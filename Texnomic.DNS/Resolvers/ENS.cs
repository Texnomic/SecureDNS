using System;
using System.Threading.Tasks;
using Nethereum.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Texnomic.DNS.Models;
using Nethereum.Web3;

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

        public async ValueTask<string> ResolveAsync(string Domain)
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
            var NameHashString = EnsUtil.GetNameHash(Domain);

            var NameHashBytes = NameHashString.HexToByteArray();

            var ResolverFunction = new ResolverFunction()
            {
                Node = NameHashBytes
            };

            //get the resolver address from ENS
            var ResolverAddress = ENSRegistryService.ResolverQueryAsync(ResolverFunction).Result;

            var ResolverService = new PublicResolverService(Web3, ResolverAddress);

            //and get the address from the resolver
            var Address = ResolverService.AddrQueryAsync(NameHashBytes).Result;

            return Address;
        }

        public byte[] Resolve(byte[] Query)
        {
            throw new NotImplementedException();
        }

        public Message Resolve(Message Query)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<Message> ResolveAsync(Message Query)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.Extensions.Options;
using Nethereum.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Texnomic.DNS.Models;
using Nethereum.Web3;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Options;
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
        private const string BaseRegistrar = "0x57f1887a8BF19b14fC0dF6Fd9B2acc9Af147eA85";
        private const string RegistryAddress = "0x00000000000C2E074eC69A0dFb2997BA6C7d2e1e";

        public ENS(IOptionsMonitor<ENSOptions> ENSOptions)
        {
            Web3 = new Web3(ENSOptions.CurrentValue.Web3.ToString());
            EnsUtil = new EnsUtil();
            ENSRegistryService = new ENSRegistryService(Web3, RegistryAddress);
            BaseRegistrarService = new BaseRegistrarService(Web3, BaseRegistrar);
            BinarySerializer = new BinarySerializer();
        }

        private async ValueTask<bool> IsAvailable(string Domain)
        {
            var Label = Domain.Split('.')[0];

            var LabelHash = EnsUtil.GetLabelHash(Label);

            var LabelHashBigInteger = LabelHash.HexToBigInteger(false);

            var AvailableFunction = new AvailableFunction()
            {
                Id = LabelHashBigInteger
            };

            return await BaseRegistrarService.AvailableQueryAsync(AvailableFunction);
        }

        private async ValueTask<string> GetAddress(string Resolver, byte[] NameHashBytes)
        {
            var ResolverService = new PublicResolverService(Web3, Resolver);

            //and get the address from the resolver
            return await ResolverService.AddrQueryAsync(NameHashBytes);
        }

        private async ValueTask<string> GetRegistrantAddress(byte[] NameHashBytes)
        {
            var OwnerFunction = new OwnerFunction()
            {
                Node = NameHashBytes
            };

            return await ENSRegistryService.OwnerQueryAsync(OwnerFunction);
        }

        private async ValueTask<string> GetResolverAddress(byte[] NameHashBytes)
        {
            var ResolverFunction = new ResolverFunction()
            {
                Node = NameHashBytes
            };

            //get the resolver address from ENS
            return await ENSRegistryService.ResolverQueryAsync(ResolverFunction);
        }

        private async ValueTask<DateTime> GetExpiryAsync(string Domain)
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
            if (!Query.Questions[0].Domain.Name.EndsWith(".eth", StringComparison.InvariantCultureIgnoreCase))
                throw new NotImplementedException($"Only .ETH Top-Level Domain is Supported.");

            var Available = await IsAvailable(Query.Questions[0].Domain.Name);

            //Checking domain availability as a workaround for missing function RecordExists in ENSRegistryService
            if (Available)
            {
                return new Message()
                {
                    ID = Query.ID,
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.NonExistentDoman,
                    Questions = Query.Questions
                };
            }

            var NameHashString = EnsUtil.GetNameHash(Query.Questions[0].Domain.Name);

            var NameHashBytes = NameHashString.HexToByteArray();

            var Resolver = await GetResolverAddress(NameHashBytes);

            var Registrant = await GetRegistrantAddress(NameHashBytes);

            var Contract = await GetAddress(Resolver, NameHashBytes);

            var TimeToLive = await GetTimeToLiveAsync(NameHashBytes);

            var Expiry = await GetExpiryAsync(Query.Questions[0].Domain.Name);

            var TXT = new TXT()
            {
                Text = new CharacterString()
                {
                    Value = Contract
                }
            };

            return new Message()
            {
                ID = Query.ID,
                MessageType = MessageType.Response,
                ResponseCode = ResponseCode.NoError,
                Questions = Query.Questions,
                Answers = new List<IAnswer>()
                {
                    new Answer()
                    {
                        Class = RecordClass.Internet,

                        Type = RecordType.TXT,

                        TimeToLive = new TimeToLive()
                        {
                            Value = TimeSpan.FromSeconds(TimeToLive)
                        },

                        Domain = Query.Questions[0].Domain,

                        Length =  TXT.Text.Length,

                        Record = TXT
                    }
                }
            };
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.Extensions.Options;
using Nethereum.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
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
    public class ENS : Protocol
    {
        private readonly Web3 Web3;
        private readonly EnsUtil EnsUtil;
        private readonly ENSService ENSService;
        private readonly ENSRegistryService ENSRegistryService;
        private readonly BaseRegistrarService BaseRegistrarService;

        public ENS(IOptionsMonitor<ENSOptions> ENSOptions, ILog Log)
        {
            Web3 = new Web3(ENSOptions.CurrentValue.Web3.ToString(), Log);
            EnsUtil = new EnsUtil();
            ENSService = new ENSService(Web3);
            ENSRegistryService = new ENSRegistryService(Web3, ENSService.EnsRegistryAddress);
            BaseRegistrarService = new BaseRegistrarService(Web3, "0x57f1887a8BF19b14fC0dF6Fd9B2acc9Af147eA85");
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

        private async ValueTask<string> GetContractAddress(string Domain)
        {
            return await ENSService.ResolveAddressAsync(Domain);
        }

        private async ValueTask<string> GetRegistrantAddress(string Domain)
        {
            var NameHashString = EnsUtil.GetNameHash(Domain);

            var NameHashBytes = NameHashString.HexToByteArray();

            var OwnerFunction = new OwnerFunction()
            {
                Node = NameHashBytes
            };

            return await ENSRegistryService.OwnerQueryAsync(OwnerFunction);
        }

        private async ValueTask<string> GetResolverAddress(string Domain)
        {
            var ResolverContract = await ENSService.GetResolverAsync(Domain);

            return ResolverContract.ContractHandler.ContractAddress;
        }

        private async ValueTask<DateTime> GetExpiryDateTimeAsync(string Domain)
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

        private async Task<ulong> GetTimeToLiveAsync(string Domain)
        {
            var NameHashString = EnsUtil.GetNameHash(Domain);

            var NameHashBytes = NameHashString.HexToByteArray();

            var TtlFunction = new TtlFunction()
            {
                Node = NameHashBytes
            };

            return await ENSRegistryService.TtlQueryAsync(TtlFunction);
        }

        public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
        {
            var Request = await BinarySerializer.DeserializeAsync<Message>(Query);

            var Response = await ResolveAsync(Request);

            return await BinarySerializer.SerializeAsync(Response);
        }

        public override async ValueTask<IMessage> ResolveAsync(IMessage Query)
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


            var ResolverAddress = await GetResolverAddress(Query.Questions[0].Domain.Name);

            var Resolver = new TXT()
            {
                Length = (byte)ResolverAddress.Length,
                Text = $"Resolver={ResolverAddress}"
            };

            var RegistrantAddress = await GetRegistrantAddress(Query.Questions[0].Domain.Name);

            var Registrant = new TXT()
            {
                Length = (byte)RegistrantAddress.Length,
                Text = $"Registrant={RegistrantAddress}"
            };

            var OwnerAddress = await GetContractAddress(Query.Questions[0].Domain.Name);

            var Owner = new TXT()
            {
                Length = (byte)OwnerAddress.Length,
                Text = $"Address={OwnerAddress}"
            };

            var ExpiryDateTime = await GetExpiryDateTimeAsync(Query.Questions[0].Domain.Name);

            var Expiry = new TXT()
            {
                Length = (byte)ExpiryDateTime.ToString().Length,
                Text = $"Expiry={ExpiryDateTime}"
            };

            var TimeToLive = await GetTimeToLiveAsync(Query.Questions[0].Domain.Name);


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

                        Length = (ushort)(Owner.Length + 1),

                        Record = Owner
                    },

                    new Answer()
                    {
                        Class = RecordClass.Internet,

                        Type = RecordType.TXT,

                        TimeToLive = new TimeToLive()
                        {
                            Value = TimeSpan.FromSeconds(TimeToLive)
                        },

                        Domain = Query.Questions[0].Domain,

                        Length = (ushort) (Resolver.Text.Length + 1),

                        Record = Resolver
                    },

                    new Answer()
                    {
                        Class = RecordClass.Internet,

                        Type = RecordType.TXT,

                        TimeToLive = new TimeToLive()
                        {
                            Value = TimeSpan.FromSeconds(TimeToLive)
                        },

                        Domain = Query.Questions[0].Domain,

                        Length = (ushort) (Registrant.Text.Length + 1),

                        Record = Registrant
                    },

                    new Answer()
                    {
                        Class = RecordClass.Internet,

                        Type = RecordType.TXT,

                        TimeToLive = new TimeToLive()
                        {
                            Value = TimeSpan.FromSeconds(TimeToLive)
                        },

                        Domain = Query.Questions[0].Domain,

                        Length = (ushort)( Expiry.Text.Length + 1),

                        Record = Expiry
                    },
                }
            };
        }

        protected override void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {

            }

            IsDisposed = true;
        }
    }
}

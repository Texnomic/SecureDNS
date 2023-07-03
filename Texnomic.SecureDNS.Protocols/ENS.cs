using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Contracts.Services;
using Nethereum.Contracts.Standards.ENS;
using Nethereum.ENS.ENSRegistry.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Texnomic.ENS.BaseRegistrar;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using Texnomic.SecureDNS.Core.Records;
using Texnomic.SecureDNS.Serialization;

using OwnerFunction = Nethereum.ENS.ENSRegistry.ContractDefinition.OwnerFunction;
using AvailableFunction =Texnomic.ENS.BaseRegistrar.ContractDefinition.AvailableFunction;
using ENSOptions = Texnomic.SecureDNS.Protocols.Options.ENSOptions;
using ENSRegistryService = Nethereum.ENS.ENSRegistryService;
using NameExpiresFunction = Texnomic.ENS.BaseRegistrar.ContractDefinition.NameExpiresFunction;

namespace Texnomic.SecureDNS.Protocols;

public class ENS : Protocol
{
    private readonly Web3 Web3;
    private readonly EnsUtil EnsUtil;
    private readonly ENSService ENSService;
    private readonly ENSRegistryService ENSRegistryService;
    private readonly BaseRegistrarService BaseRegistrarService;

    public ENS(IOptionsMonitor<ENSOptions> ENSOptions, ILogger Logger)
    {
        Web3 = new Web3(ENSOptions.CurrentValue.Web3.ToString(), Logger);
        EnsUtil = new EnsUtil();
        ENSService = new ENSService(new EthApiContractService(Web3.Client));
        ENSRegistryService = new ENSRegistryService(Web3, ENSService.EnsRegistryAddress);
        BaseRegistrarService = new BaseRegistrarService(Web3, "0x57f1887a8BF19b14fC0dF6Fd9B2acc9Af147eA85");
    }

    private async ValueTask<bool> IsAvailable(string Domain)
    {
        var Label = Domain.Split('.').First();

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
        var Label = Domain.Split('.').First();

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
        var Request =  DnSerializer.Deserialize(Query);

        var Response = await ResolveAsync(Request);

        return DnSerializer.Serialize(Response);
    }

    public override async ValueTask<IMessage> ResolveAsync(IMessage Query)
    {
        if (!Query.Questions.First().Domain.Name.EndsWith(".eth", StringComparison.InvariantCultureIgnoreCase))
            throw new NotImplementedException($"Only .ETH Top-Level Domain is Supported.");

        var Available = await IsAvailable(Query.Questions.First().Domain.Name);

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

        var Domain = Query.Questions.First().Domain.Name;

        var ResolverAddress = (TXT)$"Resolver={await GetResolverAddress(Domain)}";

        var RegistrantAddress = (TXT)$"Registrant={await GetRegistrantAddress(Domain)}";

        var ContractAddress = (TXT)$"Contract={await GetContractAddress(Domain)}";

        var ExpiryDateTime = (TXT)$"Expiry={await GetExpiryDateTimeAsync(Domain)}";

        var TimeToLive = await GetTimeToLiveAsync(Domain);

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
                    TimeToLive = TimeSpan.FromSeconds(TimeToLive),
                    Domain = Query.Questions.First().Domain,
                    Length = DnSerializer.SizeOf(ContractAddress),
                    Record = ContractAddress
                },

                new Answer()
                {
                    Class = RecordClass.Internet,
                    Type = RecordType.TXT,
                    TimeToLive = TimeSpan.FromSeconds(TimeToLive),
                    Domain = Query.Questions.First().Domain,
                    Length = DnSerializer.SizeOf(ResolverAddress),
                    Record = ResolverAddress
                },

                new Answer()
                {
                    Class = RecordClass.Internet,
                    Type = RecordType.TXT,
                    TimeToLive = TimeSpan.FromSeconds(TimeToLive),
                    Domain = Query.Questions.First().Domain,
                    Length = DnSerializer.SizeOf(RegistrantAddress),
                    Record = RegistrantAddress
                },

                new Answer()
                {
                    Class = RecordClass.Internet,
                    Type = RecordType.TXT,
                    TimeToLive = TimeSpan.FromSeconds(TimeToLive),
                    Domain = Query.Questions.First().Domain,
                    Length = DnSerializer.SizeOf(ExpiryDateTime),
                    Record = ExpiryDateTime
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
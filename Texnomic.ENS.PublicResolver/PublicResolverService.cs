using System.Numerics;
using Nethereum.Web3;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Texnomic.ENS.PublicResolver.ContractDefinition;

namespace Texnomic.ENS.PublicResolver;

public class PublicResolverService
{
    public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Web3 Web3, ABIDeployment ABIDeployment, CancellationTokenSource CancellationTokenSource = null)

    {
        return Web3.Eth.GetContractDeploymentHandler<ABIDeployment>()
            .SendRequestAndWaitForReceiptAsync(ABIDeployment, CancellationTokenSource);
    }

    public static Task<string> DeployContractAsync(Web3 Web3, ABIDeployment ABIDeployment)
    {
        return Web3.Eth.GetContractDeploymentHandler<ABIDeployment>()
            .SendRequestAsync(ABIDeployment);
    }

    public static async Task<PublicResolverService> DeployContractAndGetServiceAsync(Web3 web3, ABIDeployment ABIDeployment, CancellationTokenSource CancellationTokenSource = null)
    {
        var Receipt = await DeployContractAndWaitForReceiptAsync(web3, ABIDeployment, CancellationTokenSource);

        return new PublicResolverService(web3, Receipt.ContractAddress);
    }

    protected Web3 Web3 { get; }

    public ContractHandler ContractHandler { get; }

    public PublicResolverService(Web3 Web3, string ContractAddress)
    {
        this.Web3 = Web3;
        ContractHandler = Web3.Eth.GetContractHandler(ContractAddress);
    }

    public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction SupportsInterfaceFunction,
        BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(SupportsInterfaceFunction,
            BlockParameter);
    }


    public Task<bool> SupportsInterfaceQueryAsync(byte[] InterfaceID, BlockParameter BlockParameter = null)
    {
        var SupportsInterfaceFunction = new SupportsInterfaceFunction
        {
            InterfaceID = InterfaceID
        };

        return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(SupportsInterfaceFunction,
            BlockParameter);
    }

    public Task<string> SetDNSRecordsRequestAsync(SetDNSRecordsFunction SetDNSRecordsFunction)
    {
        return ContractHandler.SendRequestAsync(SetDNSRecordsFunction);
    }

    public Task<TransactionReceipt> SetDNSRecordsRequestAndWaitForReceiptAsync(
        SetDNSRecordsFunction SetDNSRecordsFunction, CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetDNSRecordsFunction, CancellationToken);
    }

    public Task<string> SetDNSRecordsRequestAsync(byte[] Node, byte[] Data)
    {
        var SetDNSRecordsFunction = new SetDNSRecordsFunction
        {
            Node = Node,
            Data = Data
        };

        return ContractHandler.SendRequestAsync(SetDNSRecordsFunction);
    }

    public Task<TransactionReceipt> SetDNSRecordsRequestAndWaitForReceiptAsync(byte[] Node, byte[] Data,
        CancellationTokenSource CancellationToken = null)
    {
        var SetDNSRecordsFunction = new SetDNSRecordsFunction
        {
            Node = Node,
            Data = Data
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetDNSRecordsFunction, CancellationToken);
    }

    public Task<string> SetTextRequestAsync(SetTextFunction SetTextFunction)
    {
        return ContractHandler.SendRequestAsync(SetTextFunction);
    }

    public Task<TransactionReceipt> SetTextRequestAndWaitForReceiptAsync(SetTextFunction SetTextFunction,
        CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetTextFunction, CancellationToken);
    }

    public Task<string> SetTextRequestAsync(byte[] Node, string Key, string Value)
    {
        var SetTextFunction = new SetTextFunction
        {
            Node = Node,
            Key = Key,
            Value = Value
        };

        return ContractHandler.SendRequestAsync(SetTextFunction);
    }

    public Task<TransactionReceipt> SetTextRequestAndWaitForReceiptAsync(byte[] Node, string Key, string Value,
        CancellationTokenSource CancellationToken = null)
    {
        var SetTextFunction = new SetTextFunction
        {
            Node = Node,
            Key = Key,
            Value = Value
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetTextFunction, CancellationToken);
    }

    public Task<string> InterfaceImplementerQueryAsync(InterfaceImplementerFunction InterfaceImplementerFunction,
        BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<InterfaceImplementerFunction, string>(InterfaceImplementerFunction,
            BlockParameter);
    }


    public Task<string> InterfaceImplementerQueryAsync(byte[] Node, byte[] InterfaceID,
        BlockParameter BlockParameter = null)
    {
        var InterfaceImplementerFunction = new InterfaceImplementerFunction
        {
            Node = Node,
            InterfaceID = InterfaceID
        };

        return ContractHandler.QueryAsync<InterfaceImplementerFunction, string>(InterfaceImplementerFunction,
            BlockParameter);
    }

    public Task<AbiOutputDto> AbiQueryAsync(AbiFunction ABiFunction, BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryDeserializingToObjectAsync<AbiFunction, AbiOutputDto>(ABiFunction,
            BlockParameter);
    }

    public Task<AbiOutputDto> AbiQueryAsync(byte[] Node, BigInteger ContentTypes,
        BlockParameter BlockParameter = null)
    {
        var ABiFunction = new AbiFunction
        {
            Node = Node,
            ContentTypes = ContentTypes
        };

        return ContractHandler.QueryDeserializingToObjectAsync<AbiFunction, AbiOutputDto>(ABiFunction,
            BlockParameter);
    }

    public Task<string> SetPublicKeyRequestAsync(SetPublicKeyFunction SetPublicKeyFunction)
    {
        return ContractHandler.SendRequestAsync(SetPublicKeyFunction);
    }

    public Task<TransactionReceipt> SetPublicKeyRequestAndWaitForReceiptAsync(SetPublicKeyFunction SetPublicKeyFunction,
        CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetPublicKeyFunction, CancellationToken);
    }

    public Task<string> SetPublicKeyRequestAsync(byte[] Node, byte[] X, byte[] Y)
    {
        var SetPublicKeyFunction = new SetPublicKeyFunction
        {
            Node = Node,
            X = X,
            Y = Y
        };

        return ContractHandler.SendRequestAsync(SetPublicKeyFunction);
    }

    public Task<TransactionReceipt> SetPublicKeyRequestAndWaitForReceiptAsync(byte[] Node, byte[] X, byte[] Y,
        CancellationTokenSource CancellationToken = null)
    {
        var SetPublicKeyFunction = new SetPublicKeyFunction
        {
            Node = Node,
            X = X,
            Y = Y
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetPublicKeyFunction, CancellationToken);
    }

    public Task<string> SetContenthashRequestAsync(SetContenthashFunction SetContenthashFunction)
    {
        return ContractHandler.SendRequestAsync(SetContenthashFunction);
    }

    public Task<TransactionReceipt> SetContenthashRequestAndWaitForReceiptAsync(
        SetContenthashFunction SetContenthashFunction, CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetContenthashFunction, CancellationToken);
    }

    public Task<string> SetContenthashRequestAsync(byte[] Node, byte[] Hash)
    {
        var SetContenthashFunction = new SetContenthashFunction
        {
            Node = Node,
            Hash = Hash
        };

        return ContractHandler.SendRequestAsync(SetContenthashFunction);
    }

    public Task<TransactionReceipt> SetContenthashRequestAndWaitForReceiptAsync(byte[] Node, byte[] Hash,
        CancellationTokenSource CancellationToken = null)
    {
        var SetContenthashFunction = new SetContenthashFunction
        {
            Node = Node,
            Hash = Hash
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetContenthashFunction, CancellationToken);
    }

    public Task<string> AddrQueryAsync(AddrFunction AddrFunction, BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<AddrFunction, string>(AddrFunction, BlockParameter);
    }


    public Task<string> AddrQueryAsync(byte[] Node, BlockParameter BlockParameter = null)
    {
        var AddrFunction = new AddrFunction
        {
            Node = Node
        };

        return ContractHandler.QueryAsync<AddrFunction, string>(AddrFunction, BlockParameter);
    }

    public Task<bool> HasDNSRecordsQueryAsync(HasDNSRecordsFunction HasDNSRecordsFunction,
        BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<HasDNSRecordsFunction, bool>(HasDNSRecordsFunction, BlockParameter);
    }


    public Task<bool> HasDNSRecordsQueryAsync(byte[] Node, byte[] Name, BlockParameter BlockParameter = null)
    {
        var HasDNSRecordsFunction = new HasDNSRecordsFunction
        {
            Node = Node,
            Name = Name
        };

        return ContractHandler.QueryAsync<HasDNSRecordsFunction, bool>(HasDNSRecordsFunction, BlockParameter);
    }

    public Task<string> TextQueryAsync(TextFunction TextFunction, BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<TextFunction, string>(TextFunction, BlockParameter);
    }


    public Task<string> TextQueryAsync(byte[] Node, string Key, BlockParameter BlockParameter = null)
    {
        var TextFunction = new TextFunction
        {
            Node = Node,
            Key = Key
        };

        return ContractHandler.QueryAsync<TextFunction, string>(TextFunction, BlockParameter);
    }

    public Task<string> SetAbiRequestAsync(SetAbiFunction SetAbiFunction)
    {
        return ContractHandler.SendRequestAsync(SetAbiFunction);
    }

    public Task<TransactionReceipt> SetAbiRequestAndWaitForReceiptAsync(SetAbiFunction SetAbiFunction,
        CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetAbiFunction, CancellationToken);
    }

    public Task<string> SetAbiRequestAsync(byte[] Node, BigInteger ContentType, byte[] Data)
    {
        var SetAbiFunction = new SetAbiFunction
        {
            Node = Node,
            ContentType = ContentType,
            Data = Data
        };

        return ContractHandler.SendRequestAsync(SetAbiFunction);
    }

    public Task<TransactionReceipt> SetAbiRequestAndWaitForReceiptAsync(byte[] Node, BigInteger ContentType,
        byte[] Data, CancellationTokenSource CancellationToken = null)
    {
        var SetAbiFunction = new SetAbiFunction
        {
            Node = Node,
            ContentType = ContentType,
            Data = Data
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetAbiFunction, CancellationToken);
    }

    public Task<string> NameQueryAsync(NameFunction NameFunction, BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<NameFunction, string>(NameFunction, BlockParameter);
    }


    public Task<string> NameQueryAsync(byte[] Node, BlockParameter BlockParameter = null)
    {
        var NameFunction = new NameFunction
        {
            Node = Node
        };

        return ContractHandler.QueryAsync<NameFunction, string>(NameFunction, BlockParameter);
    }

    public Task<string> SetNameRequestAsync(SetNameFunction SetNameFunction)
    {
        return ContractHandler.SendRequestAsync(SetNameFunction);
    }

    public Task<TransactionReceipt> SetNameRequestAndWaitForReceiptAsync(SetNameFunction SetNameFunction,
        CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetNameFunction, CancellationToken);
    }

    public Task<string> SetNameRequestAsync(byte[] Node, string Name)
    {
        var SetNameFunction = new SetNameFunction
        {
            Node = Node,
            Name = Name
        };

        return ContractHandler.SendRequestAsync(SetNameFunction);
    }

    public Task<TransactionReceipt> SetNameRequestAndWaitForReceiptAsync(byte[] Node, string Name,
        CancellationTokenSource CancellationToken = null)
    {
        var SetNameFunction = new SetNameFunction
        {
            Node = Node,
            Name = Name
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetNameFunction, CancellationToken);
    }

    public Task<byte[]> DnsRecordQueryAsync(DnsRecordFunction DNSRecordFunction,
        BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<DnsRecordFunction, byte[]>(DNSRecordFunction, BlockParameter);
    }


    public Task<byte[]> DnsRecordQueryAsync(byte[] Node, byte[] Name, ushort Resource,
        BlockParameter BlockParameter = null)
    {
        var DNSRecordFunction = new DnsRecordFunction
        {
            Node = Node,
            Name = Name,
            Resource = Resource
        };

        return ContractHandler.QueryAsync<DnsRecordFunction, byte[]>(DNSRecordFunction, BlockParameter);
    }

    public Task<string> ClearDNSZoneRequestAsync(ClearDNSZoneFunction ClearDNSZoneFunction)
    {
        return ContractHandler.SendRequestAsync(ClearDNSZoneFunction);
    }

    public Task<TransactionReceipt> ClearDNSZoneRequestAndWaitForReceiptAsync(ClearDNSZoneFunction ClearDNSZoneFunction, CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(ClearDNSZoneFunction, CancellationToken);
    }

    public Task<string> ClearDNSZoneRequestAsync(byte[] Node)
    {
        var ClearDNSZoneFunction = new ClearDNSZoneFunction
        {
            Node = Node
        };

        return ContractHandler.SendRequestAsync(ClearDNSZoneFunction);
    }

    public Task<TransactionReceipt> ClearDNSZoneRequestAndWaitForReceiptAsync(byte[] Node,
        CancellationTokenSource CancellationToken = null)
    {
        var ClearDNSZoneFunction = new ClearDNSZoneFunction
        {
            Node = Node
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(ClearDNSZoneFunction, CancellationToken);
    }

    public Task<byte[]> ContenthashQueryAsync(ContenthashFunction ContenthashFunction,
        BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<ContenthashFunction, byte[]>(ContenthashFunction, BlockParameter);
    }


    public Task<byte[]> ContenthashQueryAsync(byte[] Node, BlockParameter BlockParameter = null)
    {
        var ContenthashFunction = new ContenthashFunction
        {
            Node = Node
        };

        return ContractHandler.QueryAsync<ContenthashFunction, byte[]>(ContenthashFunction, BlockParameter);
    }

    public Task<PublicKeyOutputDto> PublicKeyQueryAsync(PublicKeyFunction PublicKeyFunction,
        BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryDeserializingToObjectAsync<PublicKeyFunction, PublicKeyOutputDto>(PublicKeyFunction,
            BlockParameter);
    }

    public Task<PublicKeyOutputDto> PublicKeyQueryAsync(byte[] Node, BlockParameter BlockParameter = null)
    {
        var PublicKeyFunction = new PublicKeyFunction
        {
            Node = Node
        };

        return ContractHandler.QueryDeserializingToObjectAsync<PublicKeyFunction, PublicKeyOutputDto>(PublicKeyFunction,
            BlockParameter);
    }

    public Task<string> SetAddrRequestAsync(SetAddrFunction SetAddrFunction)
    {
        return ContractHandler.SendRequestAsync(SetAddrFunction);
    }

    public Task<TransactionReceipt> SetAddrRequestAndWaitForReceiptAsync(SetAddrFunction SetAddrFunction,
        CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetAddrFunction, CancellationToken);
    }

    public Task<string> SetAddrRequestAsync(byte[] Node, string Addr)
    {
        var SetAddrFunction = new SetAddrFunction { Node = Node, Addr = Addr };

        return ContractHandler.SendRequestAsync(SetAddrFunction);
    }

    public Task<TransactionReceipt> SetAddrRequestAndWaitForReceiptAsync(byte[] Node, string Addr,
        CancellationTokenSource CancellationToken = null)
    {
        var SetAddrFunction = new SetAddrFunction
        {
            Node = Node,
            Addr = Addr
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetAddrFunction, CancellationToken);
    }

    public Task<string> SetInterfaceRequestAsync(SetInterfaceFunction SetInterfaceFunction)
    {
        return ContractHandler.SendRequestAsync(SetInterfaceFunction);
    }

    public Task<TransactionReceipt> SetInterfaceRequestAndWaitForReceiptAsync(
        SetInterfaceFunction SetInterfaceFunction, CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetInterfaceFunction, CancellationToken);
    }

    public Task<string> SetInterfaceRequestAsync(byte[] Node, byte[] InterfaceID, string Implementer)
    {
        var SetInterfaceFunction = new SetInterfaceFunction
        {
            Node = Node,
            InterfaceID = InterfaceID,
            Implementer = Implementer
        };

        return ContractHandler.SendRequestAsync(SetInterfaceFunction);
    }

    public Task<TransactionReceipt> SetInterfaceRequestAndWaitForReceiptAsync(byte[] Node, byte[] InterfaceID,
        string Implementer, CancellationTokenSource CancellationToken = null)
    {
        var SetInterfaceFunction = new SetInterfaceFunction
        {
            Node = Node,
            InterfaceID = InterfaceID,
            Implementer = Implementer
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetInterfaceFunction, CancellationToken);
    }

    public Task<bool> AuthorisationsQueryAsync(AuthorisationsFunction AuthorisationsFunction,
        BlockParameter BlockParameter = null)
    {
        return ContractHandler.QueryAsync<AuthorisationsFunction, bool>(AuthorisationsFunction, BlockParameter);
    }


    public Task<bool> AuthorisationsQueryAsync(byte[] ReturnValue1, string ReturnValue2, string ReturnValue3,
        BlockParameter BlockParameter = null)
    {
        var AuthorisationsFunction = new AuthorisationsFunction
        {
            ReturnValue1 = ReturnValue1,
            ReturnValue2 = ReturnValue2,
            ReturnValue3 = ReturnValue3
        };

        return ContractHandler.QueryAsync<AuthorisationsFunction, bool>(AuthorisationsFunction, BlockParameter);
    }

    public Task<string> SetAuthorisationRequestAsync(SetAuthorisationFunction SetAuthorisationFunction)
    {
        return ContractHandler.SendRequestAsync(SetAuthorisationFunction);
    }

    public Task<TransactionReceipt> SetAuthorisationRequestAndWaitForReceiptAsync(
        SetAuthorisationFunction SetAuthorisationFunction, CancellationTokenSource CancellationToken = null)
    {
        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetAuthorisationFunction, CancellationToken);
    }

    public Task<string> SetAuthorisationRequestAsync(byte[] Node, string Target, bool IsAuthorised)
    {
        var SetAuthorisationFunction = new SetAuthorisationFunction
        {
            Node = Node,
            Target = Target,
            IsAuthorised = IsAuthorised
        };

        return ContractHandler.SendRequestAsync(SetAuthorisationFunction);
    }

    public Task<TransactionReceipt> SetAuthorisationRequestAndWaitForReceiptAsync(byte[] Node, string Target,
        bool IsAuthorised, CancellationTokenSource CancellationToken = null)
    {
        var SetAuthorisationFunction = new SetAuthorisationFunction
        {
            Node = Node,
            Target = Target,
            IsAuthorised = IsAuthorised
        };

        return ContractHandler.SendRequestAndWaitForReceiptAsync(SetAuthorisationFunction, CancellationToken);
    }
}
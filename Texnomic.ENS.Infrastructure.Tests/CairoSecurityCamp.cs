using Nethereum.ENS;
using Nethereum.Web3;
using PublicResolverService = Texnomic.ENS.PublicResolver.PublicResolverService;

namespace Texnomic.ENS.Infrastructure.Tests;

[TestClass]
public class CairoSecurityCamp
{
    private Web3 Web3;
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
        ENSRegistryService = new ENSRegistryService(Web3, ENSRegistryAddress);
        FIFSRegistrarService = new FIFSRegistrarService(Web3, FIFSRegistrarAddress);
        PublicResolverService = new PublicResolverService(Web3, PublicResolverAddress);
    }

    // Tests commented out due to EnsUtil API changes in Nethereum 5.0
    // These tests need to be updated for the new Nethereum API
    /*
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
    */
}
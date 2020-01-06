using System.Threading.Tasks;
using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using System.Threading;
using Nethereum.Web3;
using Texnomic.ENS.BaseRegistrar.ContractDefinition;

namespace Texnomic.ENS.BaseRegistrar
{
    public class BaseRegistrarService
    {
        private Web3 Web3 { get; }

        public readonly ContractHandler ContractHandler;

        public BaseRegistrarService(Web3 Web3, string ContractAddress)
        {
            this.Web3 = Web3;
            ContractHandler = Web3.Eth.GetContractHandler(ContractAddress);
        }

        public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction SupportsInterfaceFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(SupportsInterfaceFunction, BlockParameter);
        }


        public Task<bool> SupportsInterfaceQueryAsync(byte[] InterfaceID, BlockParameter BlockParameter = null)
        {
            var SupportsInterfaceFunction = new SupportsInterfaceFunction {InterfaceID = InterfaceID};

            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(SupportsInterfaceFunction, BlockParameter);
        }

        public Task<string> GetApprovedQueryAsync(GetApprovedFunction GetApprovedFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(GetApprovedFunction, BlockParameter);
        }


        public Task<string> GetApprovedQueryAsync(BigInteger TokenId, BlockParameter BlockParameter = null)
        {
            var GetApprovedFunction = new GetApprovedFunction {TokenId = TokenId};

            return ContractHandler.QueryAsync<GetApprovedFunction, string>(GetApprovedFunction, BlockParameter);
        }

        public Task<string> ApproveRequestAsync(ApproveFunction ApproveFunction)
        {
            return ContractHandler.SendRequestAsync(ApproveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(ApproveFunction ApproveFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(ApproveFunction, CancellationToken);
        }

        public Task<string> ApproveRequestAsync(string To, BigInteger TokenId)
        {
            var ApproveFunction = new ApproveFunction {To = To, TokenId = TokenId};

            return ContractHandler.SendRequestAsync(ApproveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(string To, BigInteger TokenId, CancellationTokenSource CancellationToken = null)
        {
            var ApproveFunction = new ApproveFunction {To = To, TokenId = TokenId};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(ApproveFunction, CancellationToken);
        }

        public Task<string> TransferFromRequestAsync(TransferFromFunction TransferFromFunction)
        {
            return ContractHandler.SendRequestAsync(TransferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(TransferFromFunction TransferFromFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(TransferFromFunction, CancellationToken);
        }

        public Task<string> TransferFromRequestAsync(string From, string To, BigInteger TokenId)
        {
            var TransferFromFunction = new TransferFromFunction {From = From, To = To, TokenId = TokenId};

            return ContractHandler.SendRequestAsync(TransferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(string From, string To, BigInteger TokenId, CancellationTokenSource CancellationToken = null)
        {
            var TransferFromFunction = new TransferFromFunction {From = From, To = To, TokenId = TokenId};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(TransferFromFunction, CancellationToken);
        }

        public Task<string> ReclaimRequestAsync(ReclaimFunction ReclaimFunction)
        {
            return ContractHandler.SendRequestAsync(ReclaimFunction);
        }

        public Task<TransactionReceipt> ReclaimRequestAndWaitForReceiptAsync(ReclaimFunction ReclaimFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(ReclaimFunction, CancellationToken);
        }

        public Task<string> ReclaimRequestAsync(BigInteger ID, string Owner)
        {
            var ReclaimFunction = new ReclaimFunction {Id = ID, Owner = Owner};

            return ContractHandler.SendRequestAsync(ReclaimFunction);
        }

        public Task<TransactionReceipt> ReclaimRequestAndWaitForReceiptAsync(BigInteger ID, string Owner, CancellationTokenSource CancellationToken = null)
        {
            var ReclaimFunction = new ReclaimFunction {Id = ID, Owner = Owner};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(ReclaimFunction, CancellationToken);
        }

        public Task<string> EnsQueryAsync(EnsFunction ENSFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<EnsFunction, string>(ENSFunction, BlockParameter);
        }


        public Task<string> EnsQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<EnsFunction, string>(null, BlockParameter);
        }

        public Task<string> SafeTransferFromRequestAsync(SafeTransferFromFunction SafeTransferFromFunction)
        {
            return ContractHandler.SendRequestAsync(SafeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(SafeTransferFromFunction SafeTransferFromFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(SafeTransferFromFunction, CancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(string From, string To, BigInteger TokenId)
        {
            var SafeTransferFromFunction = new SafeTransferFromFunction {From = From, To = To, TokenId = TokenId};

            return ContractHandler.SendRequestAsync(SafeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(string From, string To, BigInteger TokenId, CancellationTokenSource CancellationToken = null)
        {
            var SafeTransferFromFunction = new SafeTransferFromFunction {From = From, To = To, TokenId = TokenId};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(SafeTransferFromFunction, CancellationToken);
        }

        public Task<BigInteger> TransferPeriodEndsQueryAsync(TransferPeriodEndsFunction TransferPeriodEndsFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferPeriodEndsFunction, BigInteger>(TransferPeriodEndsFunction, BlockParameter);
        }


        public Task<BigInteger> TransferPeriodEndsQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<TransferPeriodEndsFunction, BigInteger>(null, BlockParameter);
        }

        public Task<string> SetResolverRequestAsync(SetResolverFunction SetResolverFunction)
        {
            return ContractHandler.SendRequestAsync(SetResolverFunction);
        }

        public Task<TransactionReceipt> SetResolverRequestAndWaitForReceiptAsync(SetResolverFunction SetResolverFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(SetResolverFunction, CancellationToken);
        }

        public Task<string> SetResolverRequestAsync(string Resolver)
        {
            var SetResolverFunction = new SetResolverFunction {Resolver = Resolver};

            return ContractHandler.SendRequestAsync(SetResolverFunction);
        }

        public Task<TransactionReceipt> SetResolverRequestAndWaitForReceiptAsync(string Resolver, CancellationTokenSource CancellationToken = null)
        {
            var SetResolverFunction = new SetResolverFunction {Resolver = Resolver};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(SetResolverFunction, CancellationToken);
        }

        public Task<string> OwnerOfQueryAsync(OwnerOfFunction OwnerOfFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerOfFunction, string>(OwnerOfFunction, BlockParameter);
        }


        public Task<string> OwnerOfQueryAsync(BigInteger TokenId, BlockParameter BlockParameter = null)
        {
            var OwnerOfFunction = new OwnerOfFunction {TokenId = TokenId};

            return ContractHandler.QueryAsync<OwnerOfFunction, string>(OwnerOfFunction, BlockParameter);
        }

        public Task<BigInteger> MigrationLockPeriodQueryAsync(MigrationLockPeriodFunction MigrationLockPeriodFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<MigrationLockPeriodFunction, BigInteger>(MigrationLockPeriodFunction, BlockParameter);
        }


        public Task<BigInteger> MigrationLockPeriodQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<MigrationLockPeriodFunction, BigInteger>(null, BlockParameter);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction BalanceOfFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(BalanceOfFunction, BlockParameter);
        }


        public Task<BigInteger> BalanceOfQueryAsync(string Owner, BlockParameter BlockParameter = null)
        {
            var BalanceOfFunction = new BalanceOfFunction {Owner = Owner};

            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(BalanceOfFunction, BlockParameter);
        }

        public Task<string> RenounceOwnershipRequestAsync(RenounceOwnershipFunction RenounceOwnershipFunction)
        {
            return ContractHandler.SendRequestAsync(RenounceOwnershipFunction);
        }

        public Task<string> RenounceOwnershipRequestAsync()
        {
            return ContractHandler.SendRequestAsync<RenounceOwnershipFunction>();
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(RenounceOwnershipFunction RenounceOwnershipFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(RenounceOwnershipFunction, CancellationToken);
        }

        public Task<TransactionReceipt> RenounceOwnershipRequestAndWaitForReceiptAsync(CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<RenounceOwnershipFunction>(null, CancellationToken);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction OwnerFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(OwnerFunction, BlockParameter);
        }


        public Task<string> OwnerQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, BlockParameter);
        }

        public Task<bool> IsOwnerQueryAsync(IsOwnerFunction IsOwnerFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<IsOwnerFunction, bool>(IsOwnerFunction, BlockParameter);
        }


        public Task<bool> IsOwnerQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<IsOwnerFunction, bool>(null, BlockParameter);
        }

        public Task<bool> AvailableQueryAsync(AvailableFunction AvailableFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<AvailableFunction, bool>(AvailableFunction, BlockParameter);
        }


        public Task<bool> AvailableQueryAsync(BigInteger ID, BlockParameter BlockParameter = null)
        {
            var AvailableFunction = new AvailableFunction {Id = ID};

            return ContractHandler.QueryAsync<AvailableFunction, bool>(AvailableFunction, BlockParameter);
        }

        public Task<string> SetApprovalForAllRequestAsync(SetApprovalForAllFunction SetApprovalForAllFunction)
        {
            return ContractHandler.SendRequestAsync(SetApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(SetApprovalForAllFunction SetApprovalForAllFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(SetApprovalForAllFunction, CancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(string To, bool Approved)
        {
            var SetApprovalForAllFunction = new SetApprovalForAllFunction {To = To, Approved = Approved};

            return ContractHandler.SendRequestAsync(SetApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(string To, bool Approved, CancellationTokenSource CancellationToken = null)
        {
            var SetApprovalForAllFunction = new SetApprovalForAllFunction {To = To, Approved = Approved};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(SetApprovalForAllFunction, CancellationToken);
        }

        public Task<string> AddControllerRequestAsync(AddControllerFunction AddControllerFunction)
        {
            return ContractHandler.SendRequestAsync(AddControllerFunction);
        }

        public Task<TransactionReceipt> AddControllerRequestAndWaitForReceiptAsync(AddControllerFunction AddControllerFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(AddControllerFunction, CancellationToken);
        }

        public Task<string> AddControllerRequestAsync(string Controller)
        {
            var AddControllerFunction = new AddControllerFunction {Controller = Controller};

            return ContractHandler.SendRequestAsync(AddControllerFunction);
        }

        public Task<TransactionReceipt> AddControllerRequestAndWaitForReceiptAsync(string Controller, CancellationTokenSource CancellationToken = null)
        {
            var AddControllerFunction = new AddControllerFunction {Controller = Controller};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(AddControllerFunction, CancellationToken);
        }

        public Task<string> PreviousRegistrarQueryAsync(PreviousRegistrarFunction PreviousRegistrarFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<PreviousRegistrarFunction, string>(PreviousRegistrarFunction, BlockParameter);
        }


        public Task<string> PreviousRegistrarQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<PreviousRegistrarFunction, string>(null, BlockParameter);
        }


        public Task<string> SafeTransferFromRequestAsync(string From, string To, BigInteger TokenId, byte[] Data)
        {
            var SafeTransferFromFunction = new SafeTransferFromFunction();
            SafeTransferFromFunction.From = From;
            SafeTransferFromFunction.To = To;
            SafeTransferFromFunction.TokenId = TokenId;
            SafeTransferFromFunction.Data = Data;

            return ContractHandler.SendRequestAsync(SafeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(string From, string To, BigInteger TokenId, byte[] Data, CancellationTokenSource CancellationToken = null)
        {
            var SafeTransferFromFunction = new SafeTransferFromFunction();
            SafeTransferFromFunction.From = From;
            SafeTransferFromFunction.To = To;
            SafeTransferFromFunction.TokenId = TokenId;
            SafeTransferFromFunction.Data = Data;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(SafeTransferFromFunction, CancellationToken);
        }

        public Task<BigInteger> GracePeriodQueryAsync(GracePeriodFunction GracePeriodFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<GracePeriodFunction, BigInteger>(GracePeriodFunction, BlockParameter);
        }


        public Task<BigInteger> GracePeriodQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<GracePeriodFunction, BigInteger>(null, BlockParameter);
        }

        public Task<string> RenewRequestAsync(RenewFunction RenewFunction)
        {
            return ContractHandler.SendRequestAsync(RenewFunction);
        }

        public Task<TransactionReceipt> RenewRequestAndWaitForReceiptAsync(RenewFunction RenewFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(RenewFunction, CancellationToken);
        }

        public Task<string> RenewRequestAsync(BigInteger ID, BigInteger Duration)
        {
            var RenewFunction = new RenewFunction {Id = ID, Duration = Duration};

            return ContractHandler.SendRequestAsync(RenewFunction);
        }

        public Task<TransactionReceipt> RenewRequestAndWaitForReceiptAsync(BigInteger ID, BigInteger Duration, CancellationTokenSource CancellationToken = null)
        {
            var RenewFunction = new RenewFunction {Id = ID, Duration = Duration};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(RenewFunction, CancellationToken);
        }

        public Task<BigInteger> NameExpiresQueryAsync(NameExpiresFunction NameExpiresFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<NameExpiresFunction, BigInteger>(NameExpiresFunction, BlockParameter);
        }


        public Task<BigInteger> NameExpiresQueryAsync(BigInteger ID, BlockParameter BlockParameter = null)
        {
            var NameExpiresFunction = new NameExpiresFunction();
            NameExpiresFunction.Id = ID;

            return ContractHandler.QueryAsync<NameExpiresFunction, BigInteger>(NameExpiresFunction, BlockParameter);
        }

        public Task<bool> ControllersQueryAsync(ControllersFunction ControllersFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<ControllersFunction, bool>(ControllersFunction, BlockParameter);
        }


        public Task<bool> ControllersQueryAsync(string ReturnValue1, BlockParameter BlockParameter = null)
        {
            var ControllersFunction = new ControllersFunction();
            ControllersFunction.ReturnValue1 = ReturnValue1;

            return ContractHandler.QueryAsync<ControllersFunction, bool>(ControllersFunction, BlockParameter);
        }

        public Task<byte[]> BaseNodeQueryAsync(BaseNodeFunction BaseNodeFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<BaseNodeFunction, byte[]>(BaseNodeFunction, BlockParameter);
        }


        public Task<byte[]> BaseNodeQueryAsync(BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<BaseNodeFunction, byte[]>(null, BlockParameter);
        }

        public Task<bool> IsApprovedForAllQueryAsync(IsApprovedForAllFunction IsApprovedForAllFunction, BlockParameter BlockParameter = null)
        {
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(IsApprovedForAllFunction, BlockParameter);
        }


        public Task<bool> IsApprovedForAllQueryAsync(string Owner, string Operator, BlockParameter BlockParameter = null)
        {
            var IsApprovedForAllFunction = new IsApprovedForAllFunction {Owner = Owner, Operator = Operator};

            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(IsApprovedForAllFunction, BlockParameter);
        }

        public Task<string> AcceptRegistrarTransferRequestAsync(AcceptRegistrarTransferFunction AcceptRegistrarTransferFunction)
        {
            return ContractHandler.SendRequestAsync(AcceptRegistrarTransferFunction);
        }

        public Task<TransactionReceipt> AcceptRegistrarTransferRequestAndWaitForReceiptAsync(AcceptRegistrarTransferFunction AcceptRegistrarTransferFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(AcceptRegistrarTransferFunction, CancellationToken);
        }

        public Task<string> AcceptRegistrarTransferRequestAsync(byte[] Label, string Deed, BigInteger ReturnValue3)
        {
            var AcceptRegistrarTransferFunction = new AcceptRegistrarTransferFunction
            {
                Label = Label, Deed = Deed, ReturnValue3 = ReturnValue3
            };

            return ContractHandler.SendRequestAsync(AcceptRegistrarTransferFunction);
        }

        public Task<TransactionReceipt> AcceptRegistrarTransferRequestAndWaitForReceiptAsync(byte[] Label, string Deed, BigInteger ReturnValue3, CancellationTokenSource CancellationToken = null)
        {
            var AcceptRegistrarTransferFunction = new AcceptRegistrarTransferFunction
            {
                Label = Label, Deed = Deed, ReturnValue3 = ReturnValue3
            };

            return ContractHandler.SendRequestAndWaitForReceiptAsync(AcceptRegistrarTransferFunction, CancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction TransferOwnershipFunction)
        {
            return ContractHandler.SendRequestAsync(TransferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction TransferOwnershipFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(TransferOwnershipFunction, CancellationToken);
        }

        public Task<string> TransferOwnershipRequestAsync(string NewOwner)
        {
            var TransferOwnershipFunction = new TransferOwnershipFunction {NewOwner = NewOwner};

            return ContractHandler.SendRequestAsync(TransferOwnershipFunction);
        }

        public Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string NewOwner, CancellationTokenSource CancellationToken = null)
        {
            var TransferOwnershipFunction = new TransferOwnershipFunction {NewOwner = NewOwner};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(TransferOwnershipFunction, CancellationToken);
        }

        public Task<string> RemoveControllerRequestAsync(RemoveControllerFunction RemoveControllerFunction)
        {
            return ContractHandler.SendRequestAsync(RemoveControllerFunction);
        }

        public Task<TransactionReceipt> RemoveControllerRequestAndWaitForReceiptAsync(RemoveControllerFunction RemoveControllerFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(RemoveControllerFunction, CancellationToken);
        }

        public Task<string> RemoveControllerRequestAsync(string Controller)
        {
            var RemoveControllerFunction = new RemoveControllerFunction {Controller = Controller};

            return ContractHandler.SendRequestAsync(RemoveControllerFunction);
        }

        public Task<TransactionReceipt> RemoveControllerRequestAndWaitForReceiptAsync(string Controller, CancellationTokenSource CancellationToken = null)
        {
            var RemoveControllerFunction = new RemoveControllerFunction {Controller = Controller};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(RemoveControllerFunction, CancellationToken);
        }

        public Task<string> RegisterRequestAsync(RegisterFunction RegisterFunction)
        {
            return ContractHandler.SendRequestAsync(RegisterFunction);
        }

        public Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(RegisterFunction RegisterFunction, CancellationTokenSource CancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(RegisterFunction, CancellationToken);
        }

        public Task<string> RegisterRequestAsync(BigInteger ID, string Owner, BigInteger Duration)
        {
            var RegisterFunction = new RegisterFunction {Id = ID, Owner = Owner, Duration = Duration};

            return ContractHandler.SendRequestAsync(RegisterFunction);
        }

        public Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(BigInteger ID, string Owner, BigInteger Duration, CancellationTokenSource CancellationToken = null)
        {
            var RegisterFunction = new RegisterFunction {Id = ID, Owner = Owner, Duration = Duration};

            return ContractHandler.SendRequestAndWaitForReceiptAsync(RegisterFunction, CancellationToken);
        }
    }
}

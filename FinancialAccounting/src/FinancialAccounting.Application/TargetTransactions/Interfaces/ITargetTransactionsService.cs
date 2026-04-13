using FinancialAccouting.Contracts.TargetTransactions;

namespace FinancialAccounting.Application.TargetTransactions.Interfaces
{
    public interface ITargetTransactionsService
    {
        Task Create(CreateTargetTransactionDto targetTransactionDto, CancellationToken cancellationToken);
        Task Delete(Guid targetTransactionId, CancellationToken cancellationToken);
        Task<List<GetTargetTransactionListDto>> GetTargetTransactionsByTargetId(Guid targetId, int page, int limit, CancellationToken cancellationToken);
    }
}

using FinancialAccounting.Entities.TargetTransactions;

namespace FinancialAccounting.Application.TargetTransactions.Interfaces
{
    public interface ITargetTransactionsRepository
    {
        Task AddAsync(TargetTransaction targetTransaction, CancellationToken cancellationToken);
        Task DeleteAsync(Guid targetTransactionId, CancellationToken cancellationToken);
        Task<List<TargetTransaction>> GetTargetTransactionByTargetIdAsync(Guid targetId, int page, int limiint, CancellationToken cancellationToken);
    }
}

using FinancialAccounting.Entities.TargetTransactions;

namespace FinancialAccounting.Application.TargetTransactions.Interfaces
{
    public interface ITargetTransactionsRepository
    {
        Task AddAsync(TargetTransaction targetTransaction, CancellationToken cancellationToken);
    }
}

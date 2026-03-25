using FinancialAccounting.Entities.Transactions;

namespace FinancialAccounting.Application.Transactions
{
    public interface ITransactionsRepository
    {
        Task AddAsync(Transaction transaction, CancellationToken cancellationToken);
    }
}

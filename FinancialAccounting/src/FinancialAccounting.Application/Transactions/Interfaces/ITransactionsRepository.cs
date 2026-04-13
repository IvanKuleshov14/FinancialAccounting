using FinancialAccounting.Entities.Transactions;

namespace FinancialAccounting.Application.Transactions
{
    public interface ITransactionsRepository
    {
        Task AddAsync(Transaction transaction, CancellationToken cancellationToken);
        Task AddTransferAsync(Transaction transactionExpense, Transaction transactionIncome, CancellationToken cancellationToken);
        Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken);
        Task<List<Transaction>> GetTransactionsAsync(int page, int limit, CancellationToken cancellationToken);
    }
}

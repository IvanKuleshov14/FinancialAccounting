using FinancialAccounting.Application.Transactions;
using FinancialAccounting.Entities.Transactions;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        public Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

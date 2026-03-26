using FinancialAccounting.Application.Transactions;
using FinancialAccounting.Entities.Transactions;
using FinancialAccounting.Infrastructure.MSSQL.Data;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;

        public TransactionsRepository(FinancialAccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(transaction, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }
    }
}

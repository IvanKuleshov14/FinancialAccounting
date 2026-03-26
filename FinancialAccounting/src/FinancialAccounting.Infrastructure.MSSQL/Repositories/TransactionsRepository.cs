using FinancialAccounting.Application.Transactions;
using FinancialAccounting.Entities.Transactions;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using Microsoft.EntityFrameworkCore;

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
            var account = await _dbContext.Accounts.FindAsync(transaction.AccountId);

            if(account == null)
            {
                throw new NotImplementedException();
            }

            if(transaction.Type == TransactionTypes.Income)
            {
                account.Total += transaction.Value;
            }
            else if(transaction.Type == TransactionTypes.Expense)
            {
                account.Total -= transaction.Value;
            }

            await _dbContext.AddAsync(transaction, cancellationToken);

            await _dbContext.SaveChangesAsync();
        }
    }
}

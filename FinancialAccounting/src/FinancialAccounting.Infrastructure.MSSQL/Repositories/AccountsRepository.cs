using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Infrastructure.MSSQL.Data;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;
        public AccountsRepository(FinancialAccountingDbContext accountDbContext)
        {
            _dbContext = accountDbContext;
        }
        public async Task AddAsync(Account account, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid accountId, string accountName, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);

            if(account != null)
            {
                account.Name = accountName;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);

            if (account != null)
            {
                _dbContext.Accounts.Remove(account);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}

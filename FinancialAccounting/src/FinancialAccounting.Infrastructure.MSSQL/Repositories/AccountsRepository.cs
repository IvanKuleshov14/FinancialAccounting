using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using FinancialAccouting.Contracts.Accounts;
using Microsoft.EntityFrameworkCore;

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
            if(account == null)
            {
                throw new Exception("Счет не найден");
            }

            account.Name = accountName;
            await _dbContext.SaveChangesAsync();
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

        public async Task AddTargetAsync(AccountTarget accountTarget, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.Include(a => a.Target).FirstOrDefaultAsync(a => a.Id == accountTarget.AccountId);
            if(account == null)
            {
                throw new Exception("Счет не найден");
            }
            if(account.Target != null)
            {
                throw new Exception("У этого счета уже есть цель");
            }

            await _dbContext.AddAsync(accountTarget);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Accounts.
                Include(a => a.Target).
                AsNoTracking().
                FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}

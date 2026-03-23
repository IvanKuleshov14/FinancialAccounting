using FinancialAccounting.Application.Accounts;
using FinancialAccounting.Entities.Account;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Text;

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

        public Task DeleteAsync(Guid accountId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
    }
}

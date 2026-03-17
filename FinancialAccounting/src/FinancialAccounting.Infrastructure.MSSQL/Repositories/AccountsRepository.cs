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
        public async Task<Guid> AddAsync(Account account, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(account);
            await _dbContext.SaveChangesAsync();
            return account.Id;
        }

        public Task<Guid> DeleteAsync(Guid accountId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> SaveAsync(Account account, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

using FinancialAccounting.Entities.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Application.Accounts
{
    public interface IAccountsRepository
    {
        Task AddAsync(Account account, CancellationToken cancellationToken);
        Task DeleteAsync(Guid accountId, CancellationToken cancellationToken);
        Task UpdateAsync(Guid accountId, string accountName, CancellationToken cancellationToken);
    }
}

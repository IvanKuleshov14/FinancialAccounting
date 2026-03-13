using FinancialAccounting.Entities.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Application.Accounts
{
    public interface IAccountsRepository
    {
        Task<Guid> AddAsync(Account account, CancellationToken cancellationToken);
        Task<Guid> DeleteAsync(Guid accountId, CancellationToken cancellationToken);
        Task<Guid> SaveAsync(Account account, CancellationToken cancellationToken);
    }
}

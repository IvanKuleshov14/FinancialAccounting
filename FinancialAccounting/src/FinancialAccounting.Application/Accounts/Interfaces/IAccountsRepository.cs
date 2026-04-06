using FinancialAccounting.Entities.Accounts;

namespace FinancialAccounting.Application.Accounts
{
    public interface IAccountsRepository
    {
        Task AddAsync(Account account, CancellationToken cancellationToken);
        Task DeleteAsync(Guid accountId, CancellationToken cancellationToken);
        Task UpdateAsync(Guid accountId, string accountName, CancellationToken cancellationToken);
        Task AddTargetAsync(AccountTarget accountTarget, CancellationToken cancellationToken);
    }
}

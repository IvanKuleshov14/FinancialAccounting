using FinancialAccounting.Entities.Accounts;

namespace FinancialAccounting.Application.Accounts
{
    public interface IAccountsRepository
    {
        Task AddAsync(Account Account, CancellationToken CancellationToken);
        Task DeleteAsync(Guid AccountId, CancellationToken CancellationToken);
        Task UpdateAsync(Guid AccountId, string AccountName, CancellationToken CancellationToken);
        Task AddTargetAsync(AccountTarget AccountTarget, CancellationToken CancellationToken);
        Task<Account?> GetAccountByIdAsync(Guid Id, CancellationToken cancellationToken);
        Task<List<Account>> GetAllAccountsAsync(CancellationToken cancellationToken);
    }
}

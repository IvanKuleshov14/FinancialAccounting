using FinancialAccouting.Contracts;

namespace FinancialAccounting.Application
{
    public interface IAccountsService
    {
        Task Create(CreateAccountDto AccountDto, CancellationToken CancellationToken);
        Task Update(Guid AccountId, UpdateAccountDto AccountDto, CancellationToken CancellationToken);
        Task Delete(Guid AccountId, CancellationToken cancellationToken);
        Task CreateTarget(Guid AccountId, CreateAccountTargetDto AccountTargetDto, CancellationToken CancellationToken);
    }
}
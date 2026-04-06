using FinancialAccouting.Contracts;

namespace FinancialAccounting.Application
{
    public interface IAccountsService
    {
        Task Create(CreateAccountDto accountDto, CancellationToken cancellationToken);
        Task Update(Guid accountId, UpdateAccountDto accountDto, CancellationToken cancellationToken);
        Task Delete(Guid accountId, CancellationToken cancellationToken);
        Task CreateTarget(Guid accountId, CreateAccountTargetDto accountTargetDto, CancellationToken cancellationToken);
    }
}
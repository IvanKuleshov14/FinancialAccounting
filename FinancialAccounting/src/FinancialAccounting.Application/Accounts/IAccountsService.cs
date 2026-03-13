using FinancialAccouting.Contracts;

namespace FinancialAccounting.Application
{
    public interface IAccountsService
    {
        Task Create(CreateAccountDto accountDto, CancellationToken cancellationToken);
    }
}
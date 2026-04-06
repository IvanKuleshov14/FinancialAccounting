using FinancialAccouting.Contracts;
using FinancialAccouting.Contracts.AccountTargets;

namespace FinancialAccounting.Application.AccountTargets.Interfaces
{
    public interface IAccountTargetsService
    {
        Task Update(Guid accountTargetId, UpdateAccountTargetDto UpdateAccountTargetDto, CancellationToken cancellationToken);
    }
}

using FinancialAccouting.Contracts.Targets;

namespace FinancialAccounting.Application.Targets.Interfaces
{
    public interface ITargetsService
    {
        Task Create(CreateTargetDto targetDto, CancellationToken cancellationToken);
    }
}

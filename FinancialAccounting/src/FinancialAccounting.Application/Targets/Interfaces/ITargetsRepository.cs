using FinancialAccounting.Entities.Targets;

namespace FinancialAccounting.Application.Targets.Interfaces
{
    public interface ITargetsRepository
    {
        Task AddAsync(Target target, CancellationToken cancellationToken);
    }
}

using FinancialAccounting.Entities.Targets;

namespace FinancialAccounting.Application.Targets.Interfaces
{
    public interface ITargetsRepository
    {
        Task AddAsync(Target target, CancellationToken cancellationToken);
        Task UpdateAsync(Guid targetId, string targetName, decimal targetGoal, CancellationToken cancellationToken);
        Task DeleteAsync(Guid targetId, CancellationToken cancellationToken);
        Task<List<Target>> GetAllTargetsAsync(CancellationToken cancellationToken);
    }
}

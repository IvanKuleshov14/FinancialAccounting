using FinancialAccouting.Contracts.Targets;

namespace FinancialAccounting.Application.Targets.Interfaces
{
    public interface ITargetsService
    {
        Task Create(CreateTargetDto targetDto, CancellationToken cancellationToken);
        Task Update(Guid targetId, UpdateTargetDto updateTargetDto, CancellationToken cancellationToken);
        Task Delete(Guid targetId, CancellationToken cancellationToken);
        Task<List<GetTargetDto>> GetAllTargets(CancellationToken cancellationToken);
    }
}

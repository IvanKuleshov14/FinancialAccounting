namespace FinancialAccounting.Application.AccountTargets.Interfaces
{
    public interface IAccountTargetsRepository
    {
        Task UpdateAsync(Guid accountTargetId, string accountTargetName, decimal accountTargetGoal, CancellationToken cancellationToken);
        Task DeleteAsync(Guid accountTargetId, CancellationToken cancellationToken);
    }
}

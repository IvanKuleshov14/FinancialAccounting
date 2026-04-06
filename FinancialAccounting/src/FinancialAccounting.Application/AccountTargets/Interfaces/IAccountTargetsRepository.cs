namespace FinancialAccounting.Application.AccountTargets.Interfaces
{
    public interface IAccountTargetsRepository
    {
        Task UpdateAsync(Guid AccountTargetId, string AccountTargetName, decimal AccountTargetGoal, CancellationToken CancellationToken);
    }
}

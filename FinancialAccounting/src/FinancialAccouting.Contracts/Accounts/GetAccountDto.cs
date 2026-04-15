namespace FinancialAccouting.Contracts.Accounts
{
    public record GetAccountDto (
        Guid Id,
        string Name,
        decimal Total,
        string? TargetName,
        decimal? TargetGoal,
        decimal? TargetProgress,
        Guid? AccountTargetId
        ) {}
}

namespace FinancialAccouting.Contracts.TargetTransactions
{
    public record GetTargetTransactionListDto (
        Guid Id,
        string TargetName,
        decimal Value,
        int Type,
        string? Description,
        DateOnly CreatedDay,
        DateTime CreatedTime
        ) {}
}

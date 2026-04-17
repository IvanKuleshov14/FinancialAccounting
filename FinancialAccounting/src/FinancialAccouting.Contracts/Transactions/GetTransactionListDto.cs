namespace FinancialAccouting.Contracts.Transactions
{
    public record GetTransactionListDto(
        Guid Id,
        string AccountName,
        decimal Value,
        int Type,
        string? CategoryName,
        string? Description,
        DateOnly CreatedDay,
        DateTime CreatedTime,
        Guid? RelatedTransactionId
        )
    { }
}

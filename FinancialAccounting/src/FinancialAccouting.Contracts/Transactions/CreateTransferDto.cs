namespace FinancialAccouting.Contracts.Transactions
{
    public record CreateTransferDto(
        Guid FromAccountId,
        Guid ToAccountId,
        decimal Value,
        DateOnly CreatedDay,
        string? Description)
    { }
}

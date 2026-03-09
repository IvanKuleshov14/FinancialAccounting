namespace FinancialAccouting.Contracts
{
    public record CreateAccountDto(string Name, Guid UserId, decimal Total);
}
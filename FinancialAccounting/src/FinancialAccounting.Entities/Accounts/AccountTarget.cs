namespace FinancialAccounting.Entities.Account;

internal class AccountTarget
{
    public Guid Id { get; set; }

    public decimal Total { get; set; }

    public required decimal Goal { get; set; }
}

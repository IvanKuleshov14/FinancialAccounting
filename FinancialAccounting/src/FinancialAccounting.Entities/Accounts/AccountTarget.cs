namespace FinancialAccounting.Entities.Accounts;

public class AccountTarget
{
    public AccountTarget(Guid id, Guid accountId, string name, decimal goal)
    {
        Id = id;
        AccountId = accountId;
        Name = name;
        Goal = goal;
    }
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Account Account { get; set; } = null!;

    public string Name { get; set; }

    public decimal Goal { get; set; }
}

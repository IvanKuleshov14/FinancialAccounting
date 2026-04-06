namespace FinancialAccounting.Entities.Accounts;

public class Account
{
    public Account(Guid id, string name, Guid userId, decimal total)
    {
        Id = id;
        Name = name;
        UserId = userId;
        Total = total;
    }

    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public decimal Total { get; set; } = 0;

    public AccountTarget? Target { get; set; }
}

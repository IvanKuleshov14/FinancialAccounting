namespace FinancialAccounting.Entities.Transactions;

public class Transaction
{
    public Transaction(Guid id, Guid accountId, TransactionTypes type, decimal value, DateOnly createdDay, string? description)
    {
        Id = id;
        AccountId = accountId;
        Type = type;
        Value = value;
        CreatedDay = createdDay;
        Description = description;
    }

    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public TransactionTypes Type { get; set; }

    public Guid CategoryId { get; set; }

    public decimal Value { get; set; }

    public DateTime CreatedTime { get; set; } = DateTime.Now;

    public DateOnly CreatedDay { get; set; }

    public string? Description { get; set; }
}

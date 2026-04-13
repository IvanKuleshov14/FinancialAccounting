using FinancialAccounting.Entities.Accounts;
using FinancialAccounting.Entities.Categories;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialAccounting.Entities.Transactions;

public class Transaction
{
    public Transaction(Guid id, Guid accountId, TransactionTypes type, decimal value, DateOnly createdDay, string? description, Guid? relatedTransactionId, Guid? categoryId)
    {
        Id = id;
        AccountId = accountId;
        Type = type;
        Value = value;
        CreatedDay = createdDay;
        Description = description;
        RelatedTransactionId = relatedTransactionId;
        CategoryId = categoryId;
    }

    public Guid Id { get; set; }

    public Account Account { get; set; } = null!;

    public Guid AccountId { get; set; }

    public TransactionTypes Type { get; set; }

    public Category? Category { get; set; }

    public Guid? CategoryId { get; set; }

    public decimal Value { get; set; }

    public DateTime CreatedTime { get; set; } = DateTime.Now;

    public DateOnly CreatedDay { get; set; }

    public string? Description { get; set; }

    public Guid? RelatedTransactionId { get; set; }
}

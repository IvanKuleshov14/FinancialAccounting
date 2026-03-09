using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Entities.Incomes;

public class Transaction
{
    public Guid Id { get; set; }

    public required Guid AccountId { get; set; }

    public required TransactionTypes Type { get; set; }

    public Guid CategoryId { get; set; }

    public required decimal Value { get; set; }

    public DateTime CreatedTime { get; set; } = DateTime.Now;

    public required DateOnly CreatedDay { get; set; }

    public string? Description { get; set; }
}

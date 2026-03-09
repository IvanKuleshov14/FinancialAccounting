using FinancialAccounting.Entities.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Entities.TargetTransactions
{
    public class TargetTransaction
    {
        public Guid Id { get; set; }

        public required Guid TargetId { get; set; }

        public required TargetTransactionTypes Type { get; set; }

        public required decimal Value { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public required DateOnly CreatedDay { get; set; }

        public string? Description { get; set; }
    }
}

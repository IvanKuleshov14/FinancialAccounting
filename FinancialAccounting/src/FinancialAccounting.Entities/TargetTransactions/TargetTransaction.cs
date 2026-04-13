using FinancialAccounting.Entities.Targets;

namespace FinancialAccounting.Entities.TargetTransactions
{
    public class TargetTransaction
    {
        public TargetTransaction(Guid id, Guid targetId, TargetTransactionTypes type, decimal value, DateOnly createdDay, string? description)
        {
            Id = id;
            TargetId = targetId;
            Type = type;
            Value = value;
            CreatedDay = createdDay;
            Description = description;
        }

        public Guid Id { get; set; }

        public Target Target {  get; set; }

        public Guid TargetId { get; set; }

        public TargetTransactionTypes Type { get; set; }

        public decimal Value { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public DateOnly CreatedDay { get; set; }

        public string? Description { get; set; }
    }
}

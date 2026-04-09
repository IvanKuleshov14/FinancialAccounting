using FinancialAccounting.Entities.TargetTransactions;

namespace FinancialAccouting.Contracts.TargetTransactions
{
    public record CreateTargetTransactionDto (Guid targetId, TargetTransactionTypes type, decimal value, DateOnly createdDay, string? description) { }
}

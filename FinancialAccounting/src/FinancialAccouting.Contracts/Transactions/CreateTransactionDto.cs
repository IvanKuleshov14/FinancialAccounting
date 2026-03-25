using FinancialAccounting.Entities.Transactions;

namespace FinancialAccouting.Contracts.Transactions
{
    public record CreateTransactionDto(Guid AccountId, TransactionTypes TransactionType, decimal Value, DateOnly CreatedDay) { };
}

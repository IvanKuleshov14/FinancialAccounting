using FinancialAccouting.Contracts.TargetTransactions;

namespace FinancialAccounting.Application.TargetTransactions.Interfaces
{
    public interface ITargetTransactionsService
    {
        Task Create(CreateTargetTransactionDto targetTransactionDto, CancellationToken cancellationToken);
    }
}

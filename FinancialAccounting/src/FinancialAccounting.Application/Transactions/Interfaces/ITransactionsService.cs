using FinancialAccouting.Contracts.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Application.Transactions
{
    public interface ITransactionsService
    {
        Task Create(CreateTransactionDto transactionDto, CancellationToken cancellationToken);
        Task CreateTransfer(CreateTransferDto transferDto, CancellationToken cancellationToken);
        Task Delete(Guid transactionId, CancellationToken cancellationToken);
        Task<List<GetTransactionListDto>> GetTransactions (int page, int limit, CancellationToken cancellationToken);
    }
}

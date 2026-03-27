using FinancialAccouting.Contracts.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Application.Transactions
{
    public interface ITransactionsService
    {
        Task Create(CreateTransactionDto transactionDto, CancellationToken cancellationToken);
        Task Delete(Guid transactionId, CancellationToken cancellationToken);
    }
}

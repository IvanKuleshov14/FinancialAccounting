using FinancialAccouting.Contracts.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Application.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        public Task Create(CreateTransactionDto transactionDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

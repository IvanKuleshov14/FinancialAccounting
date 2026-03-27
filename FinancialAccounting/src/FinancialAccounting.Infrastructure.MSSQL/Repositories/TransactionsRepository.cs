using FinancialAccounting.Application.Transactions;
using FinancialAccounting.Entities.Transactions;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;

        public TransactionsRepository(FinancialAccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            var account = await _dbContext.Accounts.FindAsync(transaction.AccountId);
            if(account == null)
            {
                throw new Exception("Счет не найден");
            }

            if(transaction.Type == TransactionTypes.Income)
            {
                account.Total += transaction.Value;
            }
            else if(transaction.Type == TransactionTypes.Expense)
            {
                if(transaction.Value > account.Total)
                {
                    throw new Exception("На счете недостаточно средств");
                }
                account.Total -= transaction.Value;
            }

            await _dbContext.AddAsync(transaction, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            var transaction = await _dbContext.Transactions.FindAsync(transactionId);
            if(transaction == null)
            {
                throw new Exception("Транзакция не найдена");
            }

            var account = await _dbContext.Accounts.FindAsync(transaction.AccountId);
            if( account == null)
            {
                throw new Exception("Счет не найден");
            }

            var hasLaterTransaction = await _dbContext.Transactions.AnyAsync(
                t => t.AccountId == transaction.AccountId && t.CreatedTime > transaction.CreatedTime
                );
            if(hasLaterTransaction)
            {
                throw new Exception("Удаление невозможно - транзакция не является последней");
            }
            

            if(transaction.Type == TransactionTypes.Income)
            {
                account.Total -= transaction.Value;
            }
            else if( transaction.Type == TransactionTypes.Expense)
            {
                account.Total += transaction.Value;
            }

            _dbContext.Transactions.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}

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
            else
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

        public async Task AddTransferAsync(Transaction transactionExpense, Transaction transactionIncome, CancellationToken cancellationToken)
        {
            var fromAccount = await _dbContext.Accounts.FindAsync(transactionExpense.AccountId);
            if(fromAccount == null)
            {
                throw new Exception("Счет для списания не найден");
            }
            var toAccount = await _dbContext.Accounts.FindAsync(transactionIncome.AccountId);
            if (toAccount == null)
            {
                throw new Exception("Счет для пополнения не найден");
            }

            if(transactionExpense.Value > fromAccount.Total)
            {
                throw new Exception("На счете для списания недостаточно средств");
            }
            else
            {
                fromAccount.Total -= transactionExpense.Value;
                toAccount.Total += transactionIncome.Value;
            }

            await _dbContext.AddAsync(transactionExpense, cancellationToken);
            await _dbContext.AddAsync(transactionIncome, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            var transaction = await _dbContext.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                throw new Exception("Транзакция не найдена");
            }

            if (transaction.RelatedTransactionId == null)
            {
                var hasLaterTransaction = await _dbContext.Transactions.AnyAsync(
                    t => t.AccountId == transaction.AccountId && t.CreatedTime > transaction.CreatedTime
                    );
                if (hasLaterTransaction)
                {
                    throw new Exception("Удаление невозможно - транзакция не является последней");
                }

                var account = await _dbContext.Accounts.FindAsync(transaction.AccountId);
                if (account == null)
                {
                    throw new Exception("Счет не найден");
                }

                if (transaction.Type == TransactionTypes.Income)
                {
                    account.Total -= transaction.Value;
                }
                else if (transaction.Type == TransactionTypes.Expense)
                {
                    account.Total += transaction.Value;
                }

                _dbContext.Transactions.Remove(transaction);
            }
            else
            {
                var currentTransaction = await _dbContext.Transactions.FirstOrDefaultAsync(
                    t => t.Id == transactionId);
                if(currentTransaction == null)
                {
                    throw new Exception("Текущая транзакция не найдена");
                }
                var relatedTransaction = await _dbContext.Transactions.FirstOrDefaultAsync(
                    t => t.RelatedTransactionId == currentTransaction.RelatedTransactionId &&
                    t.Id != currentTransaction.Id);
                if (relatedTransaction == null)
                {
                    throw new Exception("Связанная транзакция не найдена");
                }

                var hasLaterCurrentTransaction = await _dbContext.Transactions.AnyAsync(
                    t => t.AccountId == currentTransaction.AccountId &&
                    t.CreatedTime > currentTransaction.CreatedTime
                    );
                var hasLaterRelatedTransaction = await _dbContext.Transactions.AnyAsync(
                    t => t.AccountId == relatedTransaction.AccountId &&
                    t.CreatedTime > relatedTransaction.CreatedTime
                    );

                if(hasLaterCurrentTransaction || hasLaterRelatedTransaction)
                {
                    throw new Exception("Удаление невозможно - транзакция не является последней");
                }

                var currentAccount = await _dbContext.Accounts.FirstOrDefaultAsync(
                    a => a.Id == currentTransaction.AccountId
                    );
                if(currentAccount == null)
                {
                    throw new Exception("Текущий счет не найден");
                }
                var relatedAccount = await _dbContext.Accounts.FirstOrDefaultAsync(
                    a => a.Id == relatedTransaction.AccountId
                    );
                if(relatedAccount == null)
                {
                    throw new Exception("Связанный счет не найден");
                }

                if (currentTransaction.Type == TransactionTypes.Income)
                {
                    currentAccount.Total -= currentTransaction.Value;
                    relatedAccount.Total += relatedTransaction.Value;
                }
                else
                {
                    currentAccount.Total += currentTransaction.Value;
                    relatedAccount.Total -= relatedTransaction.Value;
                }

                _dbContext.Transactions.Remove(currentTransaction);
                _dbContext.Transactions.Remove(relatedTransaction);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int page, int limit, CancellationToken cancellationToken)
        {
            return await _dbContext.Transactions.
                Include(t => t.Account).
                Include(t => t.Category).
                OrderByDescending(t => t.CreatedTime).
                Skip((page - 1) * limit).
                Take(limit).
                AsNoTracking().
                ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId, int page, int limit, CancellationToken cancellationToken)
        {
            return await _dbContext.Transactions.
                Include(t => t.Account).
                Include(t => t.Category).
                OrderByDescending(t => t.CreatedTime).
                Skip((page - 1) * limit).
                Take(limit).
                AsNoTracking().
                Where(t => t.AccountId == accountId).
                ToListAsync();
        }
    }
}

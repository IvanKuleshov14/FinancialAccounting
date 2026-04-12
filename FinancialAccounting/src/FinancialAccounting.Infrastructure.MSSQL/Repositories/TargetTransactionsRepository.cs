using FinancialAccounting.Application.TargetTransactions.Interfaces;
using FinancialAccounting.Entities.TargetTransactions;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class TargetTransactionsRepository : ITargetTransactionsRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;
        public TargetTransactionsRepository(FinancialAccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TargetTransaction targetTransaction, CancellationToken cancellationToken)
        {
            var target = await _dbContext.Targets.FindAsync(targetTransaction.TargetId);
            if (target == null)
            {
                throw new Exception("Цель не найдена");
            }

            if(targetTransaction.Type == TargetTransactionTypes.Income)
            {
                target.Total += targetTransaction.Value;
            }
            else
            {
                if(target.Total < targetTransaction.Value)
                {
                    throw new Exception("Расход цели не может быть больше суммы у этой цели");
                }
                target.Total -= targetTransaction.Value;
            }

            await _dbContext.AddAsync(targetTransaction, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid targetTransactionId,  CancellationToken cancellationToken)
        {
            var targetTransaction = await _dbContext.TargetTransactions.FindAsync(targetTransactionId);
            if(targetTransaction == null)
            {
                throw new Exception("Транзакция не найдена");
            }
            var hasLaterTargetTransaction = await _dbContext.TargetTransactions.AnyAsync(
                t => t.TargetId == targetTransaction.TargetId && t.CreatedTime > targetTransaction.CreatedTime);
            if(hasLaterTargetTransaction)
            {
                throw new Exception("Транзакция не является последней. Сначала удалите более ранние транзакции");
            }

            var target = await _dbContext.Targets.FindAsync(targetTransaction.TargetId);
            if(target == null)
            {
                throw new Exception("Цель не найдена");
            }

            if(targetTransaction.Type == TargetTransactionTypes.Income)
            {
                target.Total -= targetTransaction.Value;
            }
            else
            {
                target.Total += targetTransaction.Value;
            }

            _dbContext.TargetTransactions.Remove(targetTransaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}

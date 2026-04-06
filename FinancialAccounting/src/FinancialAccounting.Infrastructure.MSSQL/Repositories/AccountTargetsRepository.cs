using FinancialAccounting.Application.AccountTargets.Interfaces;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class AccountTargetsRepository : IAccountTargetsRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;
        public AccountTargetsRepository(FinancialAccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateAsync(Guid accountTargetId, string accountTargetName, decimal accountTargetGoal, CancellationToken cancellationToken)
        {
            var accountTarget = await _dbContext.AccountTargets.FirstOrDefaultAsync(t => t.Id == accountTargetId);
            if(accountTarget == null)
            {
                throw new Exception("Цель не найдена");
            }

            accountTarget.Name = accountTargetName;
            accountTarget.Goal = accountTargetGoal;

            await _dbContext.SaveChangesAsync();
        }
    }
}

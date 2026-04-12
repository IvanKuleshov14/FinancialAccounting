using FinancialAccounting.Application.Targets.Interfaces;
using FinancialAccounting.Entities.Targets;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class TargetsRepository : ITargetsRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;
        public TargetsRepository(FinancialAccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Target target, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(target);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid targetId, string targetName, decimal targetGoal, CancellationToken cancellationToken)
        {
            var target = await _dbContext.Targets.FindAsync(targetId);
            if(target == null)
            {
                throw new Exception("Цель не найдена");
            }

            target.Name = targetName;
            target.Goal = targetGoal;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid targetId, CancellationToken cancellationToken)
        {
            var target = await _dbContext.Targets.FindAsync(targetId);
            if(target == null)
            {
                throw new Exception("Цель не найдена");
            }
            _dbContext.Targets.Remove(target);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Target>> GetAllTargetsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Targets.
                AsNoTracking().
                ToListAsync();
        }
    }
}

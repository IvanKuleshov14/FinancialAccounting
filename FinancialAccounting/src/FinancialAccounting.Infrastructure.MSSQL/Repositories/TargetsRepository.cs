using FinancialAccounting.Application.Targets.Interfaces;
using FinancialAccounting.Entities.Targets;
using FinancialAccounting.Infrastructure.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}

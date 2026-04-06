using FinancialAccounting.Application.Categories.Interfaces;
using FinancialAccounting.Entities.Categories;
using FinancialAccounting.Infrastructure.MSSQL.Data;

namespace FinancialAccounting.Infrastructure.MSSQL.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly FinancialAccountingDbContext _dbContext;
        public CategoriesRepository(FinancialAccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Category category, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }
    }
}
